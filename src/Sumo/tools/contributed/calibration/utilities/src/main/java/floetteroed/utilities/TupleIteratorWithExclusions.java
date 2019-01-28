/*
 * Copyright 2015, 2016 Gunnar Flötteröd
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.utilities;

import java.util.Arrays;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;

/**
 * Iterates over all pair-wise combinations of the elements of a given
 * collection, apart from a predefined set of excluded tuples. The collection
 * must not change during the iteration.
 * 
 * 
 * @author Gunnar Flötteröd
 *
 */
public class TupleIteratorWithExclusions<E> implements Iterator<Tuple<E, E>> {

	// -------------------- MEMBERS --------------------

	private final TupleIterator<E> fullIterator;

	private final Set<Tuple<E, E>> exceptions;

	private Tuple<E, E> next = null;

	// -------------------- CONSTRUCTION --------------------

	public TupleIteratorWithExclusions(final Collection<E> elements,
			Collection<Tuple<E, E>> exceptions) {
		this.fullIterator = new TupleIterator<>(elements);
		this.exceptions = new LinkedHashSet<>(exceptions);
		this.advance();
	}

	// -------------------- INTERNALS --------------------

	private void advance() {
		this.next = null;
		while ((this.next == null) && this.fullIterator.hasNext()) {
			final Tuple<E, E> candidate = this.fullIterator.next();
			if (!this.exceptions.contains(candidate)) {
				this.next = candidate;
			}
		}
	}

	// -------------------- IMPLEMENTATION OF Iterator --------------------

	@Override
	public boolean hasNext() {
		return (this.next != null);
	}

	@Override
	public Tuple<E, E> next() {
		final Tuple<E, E> result = this.next;
		this.advance();
		return result;
	}

	@Override
	public void remove() {
		throw new UnsupportedOperationException();
	}

	// -------------------- MAIN-FUNCTION, ONLY FOR TESTING --------------------

	public static void main(String[] args) {
		final List<String> elements = Arrays.asList("a", "b", "c");
		final List<Tuple<String, String>> exclusions = Arrays.asList(
				new Tuple<String, String>("a", "c"), new Tuple<String, String>(
						"b", "c"));

		for (TupleIteratorWithExclusions<String> it = new TupleIteratorWithExclusions<>(
				elements, exclusions); it.hasNext();) {
			System.out.println(it.next());
		}
	}

}
