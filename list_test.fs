\ Tests the lists implemented in list.fs and provides some usage
\ examples.

\ List construction.

\ (1) Constructing a list in reverse order

0
list% %allot
list% %allot
list% %allot

dup constant head

list-next swap dup rot !
list-next swap dup rot !
list-next swap dup rot !

drop \ Drop the 0.

head list-length 3 = .

\ Test: list-length.

\ (1) Empty list should have length zero.

0 list-length 0= .

\ (2) Nonempty list should have correct length.

list% %allot
list% %allot

swap over list-next !
dup 0 swap list-next @ list-next !

list-length 2 = .

\ Test: append-if-new

\ (1) New clause should be appended

1 2 3 0 3 times insert-literal
0 append-if-new

dup list-length 1 = .

clauselist-clause @
1 2 3 0 3 times insert-literal
clauses-equal .

\ (2) Redundant clause should not be appended

1 2 3 0 3 times insert-literal
1 2 3 0 3 times insert-literal
0 append-if-new append-if-new

dup list-length 1 = .

clauselist-clause @
1 2 3 0 3 times insert-literal
clauses-equal .


\ Test: contains-clause.

\ (1) Contained clause should be found.

1 2 3 0 3 times insert-literal .

\ (2) Non-contained clause should not be found.



