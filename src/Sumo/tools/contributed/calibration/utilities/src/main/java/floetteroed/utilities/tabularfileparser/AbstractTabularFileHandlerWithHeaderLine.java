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
package floetteroed.utilities.tabularfileparser;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public abstract class AbstractTabularFileHandlerWithHeaderLine implements
		TabularFileHandler {

	// TODO make private
	protected final Map<String, Integer> label2index = new LinkedHashMap<String, Integer>();

	private boolean parsedFirstRow = false;

	protected int getCurrentRowSize() {
		return this.currentRow.length;
	}
	
	protected String getStringValue(final String label) {
		return this.currentRow[this.label2index.get(label)];
	}

	protected Integer getIntValue(final String label) {
		try {
			return Integer
					.parseInt(this.currentRow[this.label2index.get(label)]);
		} catch (NumberFormatException e) {
			return null;
		}
	}

	protected Double getDoubleValue(final String label) {
		try {
			return Double.parseDouble(this.currentRow[this.label2index
					.get(label)]);
		} catch (NumberFormatException e) {
			return null;
		}
	}

	// TODO new and not systematically tested
	private String[] currentRow = null;

	@Override
	public final void startRow(String[] row) {
		this.currentRow = row;
		if (!this.parsedFirstRow) {
			for (int i = 0; i < row.length; i++) {
				this.label2index.put(this.preprocessColumnLabel(row[i]), i);
			}
			this.parsedFirstRow = true;
		} else {
			this.startDataRow(row);
			this.startCurrentDataRow();
		}
	}

	@Override
	public void startDocument() {
	}

	@Override
	public String preprocess(final String line) {
		return line;
	}

	@Override
	public void endDocument() {
	}

	protected String preprocessColumnLabel(final String label) {
		return label;
	}

	protected int index(final String label) {
		return this.label2index.get(label);
	}

	public void startDataRow(final String[] row) {
	}

	// TODO NEW
	public void startCurrentDataRow() {
	}

}
