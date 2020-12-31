#!/bin/bash

dotnet build src/PartiallyApplied.sln /nologo
dotnet test src/PartiallyApplied.sln