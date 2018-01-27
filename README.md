
# Stack based languages Winter 2017

This repository contains a naive resolution theorem prover for
propositional logic written in GForth. This code was written for the
exercise part of the course *Stack Based Languages* at the Vienna
University of Technology.

## Running the Prover

The repository contains two implementations of the refutation
procedure outlined in the later sections. These implementations mainly
differ in the way they handle intermediary datastructures. The
implementations are located in the files `recursive_prover.fs` and
`iterative_prover.fs`. Both contain a word `refute` that refutes a
given clause set.

## Running the Tests

The test sources are located in the directory `tests/`. To run the
tests for a module use

```gforth <module>.fs <module>_test.fs -e bye```

A test that terminated without error usually prints `-1`; the value
`0` indicates an error.

## Input format

Clause sets are represented as in the DIMACS format. That is,
propositional variables are represented by non-zero natural numbers;
negation is expressed by the sign `-`; clauses are non-empty lists of
non-zero integers terminated by the symbol `0`, and clause sets are
represented as a sequence of clauses. For example the DIMACS
representation of the clause *x ∨ y ¬z* is `1 2 -3 0`, where the
variables *x*, *y* and *z* are represented by `1`, `2` and `3`,
respectively. Reading in a clause set can be accomplished as follows:

```
begin-dimacs

-1 0
-2 1 3 0
2 0
-3 0

4 clauses

end-dimacs
```

## Observations, Techniques and Coding Style

This section summarizes and discusses our experiences with the
developement in Forth.

### Declarative Style vs Procedural Style


In the early stages of the implementation we mostly used declarative
definitions. That means definitions were recursive and composed of
case distinctions. Even though the definitions were quite readable
they did by their size not blend in well with the conciseness of
Forth. Also as we refined some algorithms the definitions began to
accumulate case-distinctions (See The definition of `clause.merge`). At some
point it was not possible to decompose these definitions into smaller
yet meaningful words.

In order to improve the program's performance some definitions had
later to be reimplemented in a procedural style. We started again with
large definitions containing the whole algorithm for the given task.
We then have iteratively decomposed these definitions into smaller
ones. In many cases these decompositions were rather natural and
readable. For example the task of appending a new element to a list
can be decomposed as follows:

* find the list's last node.
* create a new node.
* append the node to the last element.

Apparently the programming style induced by the language Forth results
in natural and fine grained top-down decomposition. This positively
impacts the code reuse and the readability.

### Producer/Consumer Technique

Some some of the words that we have defined generate several return
values. For example the exhaustive resolution between two clauses can
generate several resolvents. For simplicity we decided to collect all
these generated elements on the stack and to leave a counter on the
top of the stack so that the next words know how many elements there
are on the stack. This induced a pattern between two words: a
*producer* and a *consumer* with the following stack effects

``` producer ( input -- product_1 ... product_n n)```

``` consumer ( product_1 ... product_n n -- output)```.

This producer-consumer pattern simplifies the implementation of words
with an unknown number of return values because it is not necessary to
carry out any memory management. Therefore this technique may also
help to avoid some sources of memory leaks. However it might result in
a more complicated implementation of the consumer. Since the stack
contains elements the consumer is limited in its ability to use the
stack. Thus the consumer needs to use local variables or the return
stack etc. Hence this pattern cannot be combined into
producer/transformer/consumer chains which are sometimes useful. This
is for instance the case for exhaustive resolution between two
clauses. A first word generates all the resolvable literals, a second
word generates the corresponding resolvents and a third word processes
the resolvents.

## Theoretical Background

We are interested in deciding whether a given formula A of
propositional logic is valid and if it is we want to obtain a proof of
its validity. We can accomplish this by reducing the problem of
proving a formula to the problem of refuting its negation. In the
following we will introduce the required definitions and give an
outline of a refutation procedure based on resolution.

### Propositional Logic

A language of propositional logic consists of a countable set *P =
{x<sub>1</sub>, x<sub>2</sub>...}* of propositional variables. Formulas are inductively
build from propositional variables and the unary connective ¬ and the
binary connectives ∧,∨, and →.


An interpretation is a function that assigns to every propositional
variable either the value 1 or the value 0. Interpretations
inductively extend to formulas by interpreting ∧ as the minimum of the
interpretation of its two arguments, ∨ as the maximum of the
interpretation of its arguments, and ¬ is interpreted as the function
which inverts the interpretation of its argument.


A formula is said to be satisfiable if there *exists* an
interpretation under which it evaluates to 1, otherwise it is
unsatisfiable. A formula is said to be valid if it evaluates to 1
under *every* interpretation.

A formula is said to be in CNF if it is a conjunction of disjunctions
of propositional variables and negations of propositional variables.
For every formula A there exists a formula B such that B is computable
in time polynomial in the size of A. Moreover A and B are equivalent with
respect to satisfiability.

The problem of deciding whether a given formula in CNF is
unsatisfiable is called **UNSAT**. This problem is coNP-complete.

### Variables, Literals, and Clauses

By a literal we mean a variable or a negated variable. A clause is the
set of literals. Formulas in CNF can thus be represented as sets of
clauses. Each clause represents a conjunct of the formula. We will
write clauses as sequents; the negative literals and positive literals
form the antecedent and the succeedent, respectively. For example the
clause *{¬x<sub>1</sub>, x<sub>2</sub>, ¬x<sub>3</sub>}* is
represented as *x<sub>1</sub>, x<sub>3</sub> → x<sub>2</sub>*.

### The Resolution Rule

Let *Γ → Δ, x* and *x, Π → Λ* be clauses where *x* is a propositional variable. The resolution inference rule can be presented as follows.

```
  Γ → Δ, x    x, Π → Λ
----------------------- (res)
       Γ, Π → Δ, Λ
```

**Lemma**: The resolution rule is sound.

**Definition**: A resolution deduction from a set of initial clauses *S* is a sequence of clauses C<sub>1</sub>, ..., C<sub>n</sub> such that every clause C<sub>i</sub> in the sequence satisfies one of the following conditions:
 1. C<sub>i</sub> is an initial clause i.e. C<sub>i</sub> ∈ S, or
 2. there exist clauses C<sub>j</sub>, C<sub>k</sub> with j, k < i such that res(C<sub>j</sub>, C<sub>k</sub>) = C<sub>i</sub>.
 
A resolution deduction C<sub>1</sub>, ..., C<sub>n</sub> from S with C<sub>n</sub> = →is called a resolution refutation of S.

**Theorem** (Soundness): Let S be a clause set. If there exists a resolution refutation of S, then S is unsatisfiable

**Theorem** (Completeness): Let S be a clause set. If S is unsatifiable, then there exists a resolution refutation of S.

**Definition** (Deductive closure): Let S be clause set, then we denote by S* the smallest superset of S closed under resolution.

Clearly with the previous definitions we have S is unsatifiable if and only if → ∈ S*, for every clause set S.

### The refutation algorithm

The considerations of the previous sections induce the following algorithm to decide the problem **UNSAT**: Given a clause set S, compute S\* by iteratively applying resolution, and stop if → is derived.
The previous algorithm clearly is correct and it terminates because S\* is finite if S is finite.

The following pseudocode is a more detailed description of the above algorithm. We partition the set S* (stricly speaking its approximation) into two sets W (working clauses) and S (seen clauses). The working clauses are the clauses that have not yet been used for a resolution, whereas the seen clauses have already been selected for resolution. By partitioning the clauses in this way we avoid some redundant inferences since exhaustive resolution is commutative.

Res(C, S) = { res<sub>x</sub>(C,D) : D ∈ S, x ∈ C, ¬x ∈ D }

```
refute(F)
  W = F  // working clauses
  S = ∅  // seen clauses
  N = ∅  // new clauses
  while → ∉ W and W ≠ ∅
    let C ∈ W
    W := W \ { C }
    S := S ∪ { C }
    N := Res(C, S) \ (W ∪ S)
    W := W ∪ N
```
