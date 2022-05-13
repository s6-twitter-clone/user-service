#!/bin/bash

tag="latest"
host="localhost:5001"
image_name="user-service"
docker_dir="user-service"
docker_context="."
skip_rebuild=false

while getopts t:h:s flag
do
    case "${flag}" in
        t) tag=${OPTARG};;
        h) host=${OPTARG};;
        s) skip_rebuild=true;;
    esac
done

if [ "$skip_rebuild" = false ]; then
    # build backend and push the image to the registry
    docker build -f $docker_dir/Dockerfile -t $host/$image_name:$tag $docker_context
    docker push $host/$image_name:$tag
fi

cd kubernetes

kubectl apply -f api.yaml -f db.yaml 