#!/bin/bash

cd kubernetes

kubectl delete -f db.yaml -f api.yaml 