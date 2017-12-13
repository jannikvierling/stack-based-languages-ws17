require util.fs
require literal.fs
require clause.fs
require clauseset.fs
require recursive_prover.fs
require parser.fs


begin-dimacs

1 2			0
-1 2		0
-2 			0

3 clauses

end-dimacs 
constant very-simple

\ verbose
\ very-simple refute

\ ----------------------------------------------

begin-dimacs

1 2			0
-1 2		0
2 3			0

3 clauses

end-dimacs 
constant very-simple2

\ very-simple2 refute

\ ----------------------------------------------

1 2 3 0		3 times insert-literal constant c0
-1 -2 4 0	3 times insert-literal constant c1

\ c0 c1 resolve-all
\ make-set-from-stack

\ Compare Haskell:
\ fibs = 0 : 1 : zipWith (+) fibs (tail fibs)
\ take 5 fibs
