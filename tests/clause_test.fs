require ../util.fs
require ../clause.fs

\ Test: clause.size

\ (1) Empty clause should have size 0.

0 clause.size 0= .

\ (2) Correct size should be computed for non-empty clause.

0
1 swap clause.insert_literal clause.size 1 = .

\ Test: clause.copy

\ (1) Empty clause should copy to 0.

0 clause.copy 0= .

\ (2) Copying non-empty clause should copy properly.

0
1 swap clause.insert_literal constant clause

clause clause.copy dup dup

clause <> .
clause.next @ 0= .
clause.literal @ 1 = .

\ Test: clause.remove_literal

\ (1) Removing from empty clause should return clause 0.

5 0 clause.remove_literal 0= .

\ (2) Removing non existant literal should not modify clause.

0
1 swap clause.insert_literal constant clause

5 clause clause.remove_literal clause = .
clause clause.next @ 0 = .
clause clause.literal @ 1 = .

\ (3) Removing existing literal from clause should remove.

1
0 1 swap clause.insert_literal clause.remove_literal 0= .

\ Test: clause.insert_literal

\ (1) Inserting non-existing literal should insert.

1 0 clause.insert_literal dup dup
0<> .
clause.next @ 0= .
clause.literal @ 1 = .


\ (2.1) Clause should be ordered after insertion.

1 0 clause.insert_literal
2 swap clause.insert_literal
clause.next @ clause.literal @ 2 = .

\ (2.2) Clause should be ordered after insertion.

1 0 clause.insert_literal
-1 swap clause.insert_literal
clause.literal @ -1 = .

\ (3) Inserting existing literal should not insert

1 0 clause.insert_literal
1 swap clause.insert_literal
clause.next @ 0= .

\ Test: merge-clause

\ (1) Merged clause should be new clause.

1 0 clause.insert_literal dup
0
clause.merge <> .

\ (2) Merged clause should not contain duplicates.

1 0 clause.insert_literal 1 0 clause.insert_literal clause.merge
clause.next @ 0 = .

\ (3) Merged clause should contain all literals from both clauses.
\ (4) Merged clause should be ordered.

1 0 clause.insert_literal 2 0 clause.insert_literal clause.merge dup
clause.literal @ 1 = .
clause.next @ clause.literal @ 2 = .

\ Test: resolve

\ (1) Resolvend should be correct

1
1 0 clause.insert_literal -1 0 clause.insert_literal clause.resolve 0= .

\ (2) Resolvent should not contain duplicates

1
1 2 0 clause.insert_literal clause.insert_literal
-1 2 0 clause.insert_literal clause.insert_literal
clause.resolve dup
clause.next @ 0= .
clause.literal @ 2 = .

\ (3) Resolvent should be ordered.

2
3 0 clause.insert_literal 2 swap clause.insert_literal
1 0 clause.insert_literal -2 swap clause.insert_literal
clause.resolve dup
clause.literal @ 1 = .
clause.next @ clause.literal @ 3 = .

\ Test: clause.equal?.

\ (1) Empty clauses should be equal.

0 0 clause.equal? . 

\ (2) Unequal clauses should not be equal

1
0 clause.insert_literal
1 2
0 clause.insert_literal clause.insert_literal
clause.equal? invert .

\ (3) Clauses with same literals should be equal

1 2 3
0 3 times clause.insert_literal
1 2 3
0 3 times clause.insert_literal
clause.equal? .

\ Test: clause.contains_literal?

\ (1) Empty list should always return false
1 0 clause.contains_literal? invert .

\ (2) Clause containing given literal should return true
1 2
0 clause.insert_literal clause.insert_literal
2 swap clause.contains_literal? .

\ (3) Clause not containing given literal should return false
1 2 0 clause.insert_literal clause.insert_literal
3 swap clause.contains_literal? invert .

\ Test: clause.resolve_full.

\ (1) Clauses having no resolvable literals should not resolve.

1 0 clause.insert_literal
2 0 clause.insert_literal
clause.resolve_full
0= .

\ (2) Clauses having multiple resolvable literals should resolve on all.

\ todo add a parsing word which repeats a parsed word a given amount of times
\ todo so we could abbrevitate clause creation 1 2 3 0 3 times clause.insert_literal

1 2 0 2 times clause.insert_literal
-1 -2 0 2 times clause.insert_literal
clause.resolve_full

2 = .

-1 1 0 clause.insert_literal clause.insert_literal  ( expected clause )
clause.equal? .

-2 2 0 clause.insert_literal clause.insert_literal  ( expected clause )
clause.equal? .

\ Test: subsumption

1 2 3 4 5 6 0 6 times clause.insert_literal
1 4 5 0 3 times clause.insert_literal
clause.subsumes_clause? invert .

1 4 5 0 3 times clause.insert_literal
1 2 3 4 5 6 0 6 times clause.insert_literal
clause.subsumes_clause? .

1 0 clause.insert_literal
1 0 clause.insert_literal
clause.subsumes_clause? .

0 0 clause.subsumes_clause? .

-1 0 clause.insert_literal
1 0 clause.insert_literal
clause.subsumes_clause? invert .

1 0 clause.insert_literal
-1 0 clause.insert_literal
clause.subsumes_clause? invert .

\ Test: clause.show.

\ (1) Empty clause should show as [ ].

0 clause.show

\ (2) Clause should display properly.

-1 1 2 3 4 5 -2
0
7 times clause.insert_literal
clause.show
