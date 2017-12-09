require clause.fs
require util.fs
require iterative_prover.fs

variable trivial

-1          0         insert-literal
-2 1 3      0 3 times insert-literal
2           0         insert-literal
-3          0         insert-literal
-1 -2 2 3 1 0 5 times insert-literal

0 5 times append-if-new trivial !

\ trivial @ main

\ Pigeon hole principle: 3 pigeons, 2 holes (UNSAT)

variable php_3^2

1 4 0 2 times insert-literal
2 5 0 2 times insert-literal
3 6 0 2 times insert-literal

-1 -4 0 2 times insert-literal
-2 -5 0 2 times insert-literal
-3 -6 0 2 times insert-literal

-1 -2 0 2 times insert-literal
-1 -3 0 2 times insert-literal
-2 -3 0 2 times insert-literal
-4 -5 0 2 times insert-literal
-4 -6 0 2 times insert-literal
-5 -6 0 2 times insert-literal

0 12 times append-if-new php_3^2 !

\ php_3^2 @ main