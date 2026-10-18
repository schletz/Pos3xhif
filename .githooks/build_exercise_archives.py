"""Builds one 7z archive per exercise project and stages it.

Every folder ``<chapter>/Uebungen/<exercise>`` becomes ``<chapter>/Uebungen/<exercise>.7z``,
so students can download an exercise without cloning the repository.

The archive is built from the git index, not from the working tree. It therefore contains
exactly what gets committed: untracked files and unstaged changes (e.g. a temporary solution
used to check the tests) never end up in the download.

Usage:
    python build_exercise_archives.py          # pre-commit mode: only changed or missing archives
    python build_exercise_archives.py --all    # rebuild every archive
"""

import shutil
import subprocess
import sys
import tempfile
from pathlib import Path, PurePosixPath

SEVEN_ZIP = Path(r"C:\Program Files\7-Zip\7z.exe")
EXCLUDES = [".vs", "bin", "obj", "packages", "TestResults", ".git"]
EXERCISE_FOLDER = "Uebungen"


def git(*args: str, input_bytes: bytes | None = None) -> bytes:
    """Runs a git command in the current repository and returns its stdout."""
    result = subprocess.run(["git", *args], input=input_bytes, capture_output=True, check=True)
    return result.stdout


def null_separated(output: bytes) -> list[str]:
    """Splits the output of a git command called with ``-z`` into paths."""
    return [p for p in output.decode("utf-8").split("\0") if p]


def exercise_of(path: str) -> PurePosixPath | None:
    """Returns ``<chapter>/Uebungen/<exercise>`` if the path lies inside an exercise folder."""
    parts = PurePosixPath(path).parts
    # A file directly in Uebungen (README.md, the archives themselves) belongs to no exercise.
    if len(parts) >= 4 and parts[1] == EXERCISE_FOLDER:
        return PurePosixPath(*parts[:3])
    return None


def find_seven_zip() -> str:
    """Returns the 7-Zip executable, preferring the default installation path."""
    if SEVEN_ZIP.exists():
        return str(SEVEN_ZIP)
    on_path = shutil.which("7z")
    if on_path is None:
        sys.exit(f"7-Zip not found at {SEVEN_ZIP} or on PATH.")
    return on_path


def build_archive(seven_zip: str, root: Path, exercise: PurePosixPath) -> Path:
    """Exports the staged files of one exercise to a temp folder and packs them."""
    archive = root / f"{exercise}.7z"
    with tempfile.TemporaryDirectory() as tmp:
        prefix = Path(tmp) / "export"
        files = git("ls-files", "-z", "--", str(exercise))
        # checkout-index writes the index version of each file below the prefix,
        # keeping its path relative to the repository root.
        git("checkout-index", "-z", "--stdin", f"--prefix={prefix.as_posix()}/", input_bytes=files)
        new_archive = Path(tmp) / archive.name
        excludes = [f"-xr!{name}" for name in EXCLUDES]
        # Running 7z in the parent folder makes <exercise>/ the top-level folder of the archive.
        subprocess.run(
            [seven_zip, "a", "-mx=9", str(new_archive), exercise.name, *excludes],
            cwd=prefix / exercise.parent, capture_output=True, check=True,
        )
        # "7z a" would update an existing archive and keep deleted files, so replace it.
        shutil.move(new_archive, archive)
    return archive


def main() -> None:
    root = Path(git("rev-parse", "--show-toplevel").decode().strip())
    rebuild_all = "--all" in sys.argv[1:]

    tracked = null_separated(git("ls-files", "-z"))
    exercises = sorted({e for p in tracked if (e := exercise_of(p)) is not None})
    staged = {e for p in null_separated(git("diff", "--cached", "--name-only", "-z"))
              if (e := exercise_of(p)) is not None}

    seven_zip = find_seven_zip()
    for exercise in exercises:
        archive = root / f"{exercise}.7z"
        if rebuild_all or exercise in staged or not archive.exists():
            build_archive(seven_zip, root, exercise)
            git("add", "--", str(archive.relative_to(root).as_posix()))
            print(f"Built {exercise}.7z")

    # An exercise that was removed from the index must not leave its archive behind.
    for archive_path in tracked:
        path = PurePosixPath(archive_path)
        if (path.suffix == ".7z" and len(path.parts) == 3 and path.parts[1] == EXERCISE_FOLDER
                and path.with_suffix("") not in exercises):
            git("rm", "--quiet", "--", archive_path)
            print(f"Removed {archive_path}")


if __name__ == "__main__":
    try:
        main()
    except subprocess.CalledProcessError as error:
        sys.exit(f"Building exercise archives failed: {error}\n{error.stderr.decode(errors='replace')}")
