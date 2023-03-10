/*
 * Source of timetable-code: https://github.com/Grible/timetable.js
 * Outdated javascript-plugin was modified to fulfill the requirements of our timetable-view.
 * 
 * GPL-3.0 license applies to "timetable.js"
 */

'use strict';

var Timetable = function () {
	this.scope = {
		hourStart: 9,
		hourEnd: 17,
		date: null
	};
	this.locations = [];
	this.events = [];
};

Timetable.Renderer = function (tt) {
	if (!(tt instanceof Timetable)) {
		throw new Error('Initialize renderer using a Timetable');
	}
	this.timetable = tt;
};

(function () {
	function isValidHourRange(start, end) {
		return isValidHour(start) && isValidHour(end);
	}
	function isValidHour(number) {
		return isInt(number) && isInHourRange(number);
	}
	function isInt(number) {
		return number === parseInt(number, 10);
	}
	function isInHourRange(number) {
		return number >= 0 && number < 25;
	}
	function locationExistsIn(loc, locs) {
		for (var k = 0; k < locs.length; k++) {
			if (loc === locs[k].id) {
				return true;
			}
		}
		return false;
	}
	function isValidTimeRange(start, end) {
		var correctTypes = start instanceof Date && end instanceof Date;
		var correctOrder = start < end;
		return correctTypes && correctOrder;
	}
	function getDurationHours(startHour, endHour) {
		return endHour >= startHour ? endHour - startHour : 25 + endHour - startHour;
	}

	Timetable.prototype = {
		setScope: function (start, end, date) {
			if (isValidHourRange(start, end)) {
				this.scope.hourStart = start;
				this.scope.hourEnd = end;
				this.scope.date = date
			} else {
				throw new RangeError('Timetable scope should consist of (start, end) in whole hours from 0 to 24');
			}

			return this;
		},
		addLocations: function (newLocations) {
			function hasProperFormat() {
				return newLocations instanceof Array && typeof newLocations[0] === 'string';
			}

			function hasExtendFormat() {
				return newLocations instanceof Array && newLocations[0] instanceof Object;
			}

			var existingLocations = this.locations;

			if (hasProperFormat()) {
				newLocations.forEach(function (loc) {
					if (!locationExistsIn(loc, existingLocations)) {
						existingLocations.push({
							id: loc,
							title: loc
						});
					} else {
						throw new Error('Location already exists');
					}
				});
			} else if (hasExtendFormat()) {
				newLocations.forEach(function (loc) {
					if (!locationExistsIn(loc, existingLocations)) {
						existingLocations.push(loc);
					} else {
						throw new Error('Location already exists');
					}
				});
			}
			else {
				throw new Error('Tried to add locations in wrong format');
			}

			return this;
		},
		addEvent: function (name, location, start, end, options) {
			if (!locationExistsIn(location, this.locations)) {
				throw new Error('Unknown location');
			}
			if (!isValidTimeRange(start, end)) {
				console.log('Invalid time range: ' + JSON.stringify([start, end]));
				return;
			}

			var optionsHasValidType = Object.prototype.toString.call(options) === '[object Object]';

			this.events.push({
				name: name,
				location: location,
				startDate: start,
				endDate: end,
				options: optionsHasValidType ? options : undefined
			});

			return this;
		}
	};

	function emptyNode(node) {
		while (node.firstChild) {
			node.removeChild(node.firstChild);
		}
	}

	function prettyFormatHour(hour) {
		var prefix = hour < 10 ? '0' : '';
		return prefix + hour + ':00';
	}

	Timetable.Renderer.prototype = {
		draw: function (selector) {
			function checkContainerPrecondition(container) {
				if (container === null) {
					throw new Error('Timetable container not found');
				}
			}
			function appendTimetableAside(container) {
				var asideNode = container.appendChild(document.createElement('aside'));
				var asideULNode = asideNode.appendChild(document.createElement('ul'));
				appendRowHeaders(asideULNode);
			}
			function appendRowHeaders(ulNode) {
				for (var k = 0; k < timetable.locations.length; k++) {
					var url = timetable.locations[k].href;
					var actionUrl = timetable.locations[k].actionUrl;
					var liNode = ulNode.appendChild(document.createElement('li'));

					var rowNameDiv = liNode.appendChild(document.createElement('div'));
					rowNameDiv.className = 'row-name-div';
					var rowNameSpan = rowNameDiv.appendChild(document.createElement('span'));
					rowNameSpan.className = 'row-heading';
					rowNameSpan.textContent = timetable.locations[k].title;
					if (url !== undefined) {
						var aNode = liNode.appendChild(document.createElement('a'));
						aNode.href = timetable.locations[k].href;
						aNode.appendChild(rowNameSpan);
					}

					if (actionUrl !== undefined) {
						var rowActionDiv = liNode.appendChild(document.createElement('div'));
						rowActionDiv.className = 'row-action-div';

						var actionLink = rowActionDiv.appendChild(document.createElement('a'));
						actionLink.className = 'row-action-button';
						actionLink.href = actionUrl;

						var actionImg = actionLink.appendChild(document.createElement('i'));
						actionImg.className = 'fa fa-solid fa-plus-circle';
                    }					
				}
			}
			function navigateTo(loc) {
				location.href = loc;
            }
			function appendTimetableSection(container) {
				var sectionNode = container.appendChild(document.createElement('section'));
				var timeNode = sectionNode.appendChild(document.createElement('time'));
				appendColumnHeaders(timeNode);
				appendTimeRows(timeNode);
			}
			function appendColumnHeaders(node) {
				var headerNode = node.appendChild(document.createElement('header'));
				var headerULNode = headerNode.appendChild(document.createElement('ul'));

				var completed = false;
				var looped = false;

				for (var hour = timetable.scope.hourStart; !completed;) {
					var liNode = headerULNode.appendChild(document.createElement('li'));
					var spanNode = liNode.appendChild(document.createElement('span'));
					spanNode.className = 'time-label';
					spanNode.textContent = prettyFormatHour(hour);

					if (hour === timetable.scope.hourEnd && (timetable.scope.hourStart !== timetable.scope.hourEnd || looped)) {
						completed = true;
					}
					if (++hour === 25) {
						hour = 0;
						looped = true;
					}
				}
			}
			function appendTimeRows(node) {
				var ulNode = node.appendChild(document.createElement('ul'));
				ulNode.className = 'room-timeline';
				for (var k = 0; k < timetable.locations.length; k++) {
					var liNode = ulNode.appendChild(document.createElement('li'));
					appendLocationEvents(timetable.locations[k], liNode);/**/
				}
			}
			function appendLocationEvents(location, node) {
				for (var k = 0; k < timetable.events.length; k++) {
					var event = timetable.events[k];
					if (event.location === location.id) {
						appendEvent(event, node);
					}
				}
			}
			function appendEvent(event, node) {
				var hasOptions = event.options !== undefined;
				var hasURL, hasAdditionalClass, hasDataAttributes = false;

				if (hasOptions) {
					hasURL = event.options.url !== undefined;
					hasAdditionalClass = event.options.class !== undefined;
					hasDataAttributes = event.options.data !== undefined;
				}

				var elementType = hasURL ? 'a' : 'span';
				var aNode = node.appendChild(document.createElement(elementType));
				var smallNode = aNode.appendChild(document.createElement('small'));
				aNode.title = event.name;

				if (hasURL) {
					aNode.href = event.options.url;
				}
				if (hasDataAttributes) {
					for (var key in event.options.data) {
						aNode.setAttribute('data-' + key, event.options.data[key]);
					}
				}

				aNode.className = hasAdditionalClass ? 'time-entry ' + event.options.class : 'time-entry';
				aNode.style.width = computeEventBlockWidth(event);
				aNode.style.left = computeEventBlockOffset(event);
				smallNode.textContent = event.name;
			}
			function computeEventBlockWidth(event) {
				var start = event.startDate;
				var end = event.endDate;
				var durationHours = 0;

				if (moment(event.startDate).isBefore(timetable.scope.date, 'day')) {
					var startDate = timetable.scope.date.toDate();
					start.setHours(timetable.scope.hourStart);
					start.setMinutes(0);
					start.setDate(startDate.getDate());
					start.setMonth(startDate.getMonth());
					start.setFullYear(startDate.getFullYear());
					durationHours = computeDurationInHours(start, end);
				}
				else {
					durationHours = computeDurationInHours(start, end);
                }				

				if (moment(event.endDate).isAfter(timetable.scope.date, 'day')) {
					durationHours = timetable.scope.hourEnd;
				}
				return durationHours / scopeDurationHours * 100 + '%';
				
			}
			function computeDurationInHours(start, end) {
				return (end.getTime() - start.getTime()) / 1000 / 60 / 60;
			}
			function computeEventBlockOffset(event) {
				var scopeStartHours = timetable.scope.hourStart;			
				var eventStartHours = event.startDate.getHours() + (event.startDate.getMinutes() / 60);

				if (moment(event.startDate).isBefore(timetable.scope.date, 'day')) {
					eventStartHours = timetable.scope.hourStart;
				}

				var hoursBeforeEvent = getDurationHours(scopeStartHours, eventStartHours);
				return hoursBeforeEvent / scopeDurationHours * 100 + '%';
			}

			var timetable = this.timetable;
			var scopeDurationHours = getDurationHours(timetable.scope.hourStart, timetable.scope.hourEnd);
			var container = document.querySelector(selector);
			checkContainerPrecondition(container);
			emptyNode(container);
			appendTimetableAside(container);
			appendTimetableSection(container);
		}
	};

})();
