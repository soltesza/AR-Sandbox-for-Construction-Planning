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
package floetteroed.utilities.statisticslogging;

import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class TimeStampStatistic<D extends Object> implements Statistic<D> {

	public static final String TIMESTAMP = "Timestamp";

	private final String label;

	public TimeStampStatistic() {
		this.label = TIMESTAMP;
	}

	public TimeStampStatistic(final String label) {
		this.label = label;
	}

	@Override
	public String label() {
		return TIMESTAMP;
	}

	@Override
	public String value(final D data) {
		return (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss")).format(new Date(
				System.currentTimeMillis()));
	}
}
