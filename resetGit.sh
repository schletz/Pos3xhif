#!/bin/bash

for branch in $(git branch | tr '*' ' ')
do
    echo Reset branch $branch
    git checkout %%a
    git clean -df
    git reset --hard origin/$branch
done
