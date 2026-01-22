#!/bin/sh
docker build -t lowsharp.server:$(date +%Y.%m.%d) .
