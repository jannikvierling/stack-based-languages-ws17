: times ( n "name" -- )
    \ Executes the parsed word n times.
    ' { xt } 0 u+do xt execute loop ;
