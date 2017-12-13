require util.fs
require literal.fs
require clause.fs
require clauseset.fs
require recursive_prover.fs

variable example

1 2 3 0   3 times insert-literal
1 -2 4 0  3 times insert-literal
-1 -2 4 0 3 times insert-literal
1 2 0     2 times insert-literal
-1 2 0    2 times insert-literal
-2 0              insert-literal

0         6 times insert-clause

example !

\ is-sat invert .
