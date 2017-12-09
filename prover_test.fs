require clause.fs
require prover.fs
require util.fs

-1 0 insert-literal
dup is-not-seen .

-2 1 3 0 3 times insert-literal
dup is-not-seen .

2 0 insert-literal
dup is-not-seen .

-3 0 insert-literal
dup is-not-seen .

\ Irrelevant clause (first four clauses are unsat)
-1 -2 2 3 1 0 5 times insert-literal

0 5 times append-if-new

\ status 
main