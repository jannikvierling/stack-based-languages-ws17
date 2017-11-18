\ Test: clause-size

\ (1) Empty clause should have size 0.

0 clause-size 0= .

\ (2) Correct size should be computed for non-empty clause.

0
1 swap insert-literal clause-size 1 = .

\ Test: copy-clause

\ (1) Empty clause should copy to 0.

0 copy-clause 0= .

\ (2) Copying non-empty clause should copy properly.

0
1 swap insert-literal constant clause

clause copy-clause dup dup

clause <> .
clause-next @ 0= .
clause-literal @ 1 = .

\ Test: remove-literal

\ (1) Removing from empty clause should return clause 0.

5 0 remove-literal 0= .

\ (2) Removing non existant literal should not modify clause.

0
1 swap insert-literal constant clause

5 clause remove-literal clause = .
clause clause-next @ 0 = .
clause clause-literal @ 1 = .

\ (3) Removing existing literal from clause should remove.

1
0 1 swap insert-literal remove-literal 0= .

\ Test: insert-literal

\ (1) Inserting non-existing literal should insert.

1 0 insert-literal dup dup
0<> .
clause-next @ 0= .
clause-literal @ 1 = .


\ (2.1) Clause should be ordered after insertion.

1 0 insert-literal
2 swap insert-literal
clause-next @ clause-literal @ 2 = .

\ (2.2) Clause should be ordered after insertion.

1 0 insert-literal
-1 swap insert-literal
clause-literal @ -1 = .

\ (3) Inserting existing literal should not insert

1 0 insert-literal
1 swap insert-literal
clause-next @ 0= .

\ Test: merge-clause

\ (1) Merged clause should be new clause.

1 0 insert-literal dup
0
merge-clauses <> .

\ (2) Merged clause should not contain duplicates.

1 0 insert-literal 1 0 insert-literal merge-clauses
clause-next @ 0 = .

\ (3) Merged clause should contain all literals from both clauses.
\ (4) Merged clause should be ordered.

1 0 insert-literal 2 0 insert-literal merge-clauses dup
clause-literal @ 1 = .
clause-next @ clause-literal @ 2 = .

\ Test: resolve

\ (1) Resolvend should be correct

1
1 0 insert-literal -1 0 insert-literal resolve-clauses 0= .

\ (2) Resolvent should not contain duplicates

1
1 0 insert-literal 2 swap insert-literal
-1 0 insert-literal 2 swap insert-literal
resolve-clauses dup
clause-next @ 0= .

\ (3) Resolvent should be ordered.

2
3 0 insert-literal 2 swap insert-literal
1 0 insert-literal -2 swap insert-literal
resolve-clauses dup
clause-literal @ 1 = .
clause-next @ clause-literal @ 3 = .

\ Test: clauses-equal.

\ (1) Empty clauses should be equal.

0 0 clauses-equal . 

\ (2) Unequal clauses should not be equal

1
0 insert-literal
1 2
0 insert-literal insert-literal
clauses-equal invert .

\ (3) Clauses with same literals should be equal
1 2
0 insert-literal insert-literal
1 2
0 insert-literal insert-literal
clauses-equal .

\ Test: show-clause.

\ (1) Empty clause should show as [ ].

0 show-clause

\ (2) Clause should display properly.

-1 1 2 3 4 5 -2
0
insert-literal insert-literal
insert-literal insert-literal
insert-literal insert-literal
insert-literal
show-clause
