#!/bin/bash
# Setzt alle lokalen Branches zurück

current_branch=$(git branch --show-current)
for branch in $(git branch | tr '*' ' ')
do
    echo Reset branch $branch
    git checkout $branch
    git clean -df
    git reset --hard origin/$branch
done

git checkout $current_branch
echo "You are in Branch $current_branch"
