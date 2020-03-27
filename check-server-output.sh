#! /bin/bash

if grep -q Exception server-output.txt; then
    has_exception=1;
else
    has_exception=0;
fi

if [[ $has_exception -eq 1 ]]; then
    cat server-output.txt
    echo "================================================="
    echo "Exist exception in server log."
    echo "================================================="
fi
test $has_exception -eq 0