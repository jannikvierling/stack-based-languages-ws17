
# Stack based languages Winter 2017

This repository contains a naive implementation of resolution theorem proving for propositional logic in GForth.

## Compiling

## Running the Tests

## Usage

## Examples

## Theory   

### Variables, Literals, and Clauses

We denote propositional variables by *x<sub>1</sub>*, *x<sub>3</sub>*,  *x<sub>3</sub>*, etc.. A literal is a propositional variable or its negation. Clauses are sets of literals. Clauses will be represented as sequents; the negative literals and positive literals form the antecedent and the succeedent, respectively. For example the clause *{¬x<sub>1</sub>, x<sub>2</sub>, ¬x<sub>3</sub>}* is represented as *x<sub>1</sub>, x<sub>3</sub> → x<sub>2</sub>*.

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

The considerations of the previous sections induce the following algorithm to decide unsatisfiability of clause sets: Given a clause set S, compute S* by iteratively applying resolution, and stop if → is derived.
The previous algorithm clearly is correct and it terminates because S* is finite if S is finite.

The following pseudocode is a more detailed description of the above algorithm. We partition the set S* (stricly speaking its approximation) into two sets W (working clauses) and S (seen clauses). The working clauses are the clauses that have not yet been used for a resolution, whereas the seen clauses have already been selected for resolution. By partitioning the clauses in this way we avoid some redudant inferences since exhaustive resolution is commutative.

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

### Correctness and Termination
