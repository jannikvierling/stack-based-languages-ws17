require ../parser.fs
require ../iterative_prover.fs

\ A trivial example.

variable trivial

begin-dimacs

-1          0
-2 1 3      0
2           0
-3          0
-1 -2 2 3 1 0

5 clauses

end-dimacs trivial !

\ trivial @ main

\ Pigeon hole principle: 3 pigeons, 2 holes (UNSAT)

variable php_3^2

begin-dimacs

1 4 0 
2 5 0 
3 6 0 

-1 -4 0 
-2 -5 0 
-3 -6 0 

-1 -2 0 
-1 -3 0 
-2 -3 0 
-4 -5 0 
-4 -6 0 
-5 -6 0 

12 clauses

end-dimacs php_3^2 !

\ php_3^2 @ main

\ Pigeon hole principle with 4 pigeons, 2 holes
\ created with cnfgen

variable php_4^2

begin-dimacs

1 2 0
3 4 0
5 6 0
7 8 0
-1 -3 0
-1 -5 0
-1 -7 0
-3 -5 0
-3 -7 0
-5 -7 0
-2 -4 0
-2 -6 0
-2 -8 0
-4 -6 0
-4 -8 0
-6 -8 0

16 clauses

end-dimacs php_4^2 !

\ Pigeon hole principle with 4 pigeons, 2 holes
\ created with cnfgen

variable php_4^3

begin-dimacs

1 2 3 0
4 5 6 0
7 8 9 0
10 11 12 0
-1 -4 0
-1 -7 0
-1 -10 0
-4 -7 0
-4 -10 0
-7 -10 0
-2 -5 0
-2 -8 0
-2 -11 0
-5 -8 0
-5 -11 0
-8 -11 0
-3 -6 0
-3 -9 0
-3 -12 0
-6 -9 0
-6 -12 0
-9 -12 0

22 clauses

end-dimacs php_4^3 !