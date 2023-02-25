#!/bin/bash
# Setzt alle lokalen Branches zurück

git fetch --all --prune
current_branch=$(git branch --show-current)
for branch in $(git branch | tr '*' ' ')
do
    echo Reset branch $branch
    git checkout $branch &> /dev/null
    # git clean -df
    git reset --hard origin/$branch &> /dev/null
done

git checkout $current_branch &> /dev/null
echo "You are in Branch $current_branch" &> /dev/null
