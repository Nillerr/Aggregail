#!/usr/bin/env bash

bootstrap_dist_dir=./node_modules/bootstrap/dist/css
bootswatch_dist_dir=./node_modules/bootswatch/dist

bootstrap_file=bootstrap.min.css
public_dir=./public/themes

mkdir -p "$public_dir/bootstrap"
cp "$bootstrap_dist_dir/$bootstrap_file" "$public_dir/bootstrap/$bootstrap_file";

for dir in "$bootswatch_dist_dir"/*; do
  if [ -d "$dir" ]; then
    name="${dir##*/}"
    mkdir -p "$public_dir/$name"
    cp "$bootswatch_dist_dir/$name/$bootstrap_file" "$public_dir/$name/$bootstrap_file"
  fi
done
