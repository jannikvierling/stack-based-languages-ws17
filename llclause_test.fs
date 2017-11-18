0 constant clause

\ Empty clause should have size 0.
clause clause-size 0= .

\ Empty clause should copy to 0.
clause copy-clause 0= .

\ Removing from empty clause should return clause 0.
5 clause remove-literal 0= .

clause% %allot constant clause

0 clause clause-next !
1 clause clause-literal !

\ Correct size should be computed for non-empty clause.
clause clause-size 1 = .

\ Copying non-empty clause should copy properly.
clause copy-clause dup

clause-next @ 0= .
clause-literal @ 1 = .

\ Removing literal existing literal from clause should remove.

1 clause remove-literal 0= .

\ Inserting literals into clause

0 
1 swap insert-literal
2 swap insert-literal
3 swap insert-literal
-1 swap insert-literal
2 swap insert-literal

0
4 swap insert-literal
5 swap insert-literal
-2 swap insert-literal

merge-clauses

0
1 swap insert-literal
2 swap insert-literal
3 swap insert-literal
-1 swap insert-literal

0
4 swap insert-literal
5 swap insert-literal
-2 swap insert-literal

2 rot rot resolve-clauses

show-clause
