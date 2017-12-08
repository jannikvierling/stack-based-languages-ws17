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

4 dispatch-new

working @ list-length 4 = .