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
import java.util.List;

/**
 * Iterates over all pair-wise combinations of the elements of a given
 * collection. This collection must not change during the iteration.
 * 
 * @author Gunnar Flötteröd
 *
 */
public class TupleIterator<E> implements Iterator<Tuple<E, E>> {

	// -------------------- MEMBERS --------------------

	private final Collection<E> elements;

	private Iterator<E> firstElementIterator = null;

	private Iterator<E> secondElementIterator = null;

	private E firstElement = null;

	// -------------------- CONSTRUCTION --------------------

	public TupleIterator(final Collection<E> elements) {
		this.elements = elements;
		this.firstElementIterator = elements.iterator();
		this.secondElementIterator = elements.iterator();
		if (this.firstElementIterator.hasNext()) {
			this.firstElement = this.firstElementIterator.next();
		}
	}

	// -------------------- IMPLEMENTATION OF Iterator --------------------

	@Override
	public boolean hasNext() {
		// note that this returns false if this.elements is empty
		return (this.firstElementIterator.hasNext() || this.secondElementIterator
				.hasNext());
	}

	@Override
	public Tuple<E, E> next() {
		if (!this.secondElementIterator.hasNext()) {
			this.firstElement = this.firstElementIterator.next();
			this.secondElementIterator = this.elements.iterator();
		}
		return new Tuple<E, E>(this.firstElement,
				this.secondElementIterator.next());
	}

	@Override
	public void remove() {
		throw new UnsupportedOperationException();
	}

	// -------------------- MAIN-FUNCTION, ONLY FOR TESTING --------------------

	public static void main(String[] args) {
		final List<String> elements = Arrays.asList("a", "b", "c");
		for (TupleIterator<String> it = new TupleIterator<>(elements); it
				.hasNext();) {
			System.out.println(it.next());
		}
	}
}
