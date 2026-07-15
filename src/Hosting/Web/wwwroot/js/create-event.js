function updateCharCount(inputId, countId, max) {
    var input = document.getElementById(inputId);
    var countEl = document.getElementById(countId);
    if (!input || !countEl) return;
    var length = input.value.length;
    countEl.textContent = length > max ? max : length;
}

function updatePreviewTitle() {
    var titleInput = document.getElementById('titleInput');
    var previewTitle = document.getElementById('previewTitle');
    if (!titleInput || !previewTitle) return;
    previewTitle.textContent = titleInput.value.trim() || 'Event Title';
}

function selectEventType(value) {
    document.querySelectorAll('.event-type-card').forEach(function (card) {
        card.classList.remove('selected');
    });

    var radio = document.querySelector('input[data-event-type-radio][value="' + value + '"]');
    if (radio) {
        var card = radio.closest('.event-type-card');
        radio.checked = true;
        card.classList.add('selected');

        var previewEventType = document.getElementById('previewEventType');
        var labelEl = card.querySelector('span:last-of-type');
        if (previewEventType && labelEl) {
            previewEventType.textContent = labelEl.textContent;
        }
    }
}

function previewCoverImage(event) {
    var file = event.target.files && event.target.files[0];
    var img = document.getElementById('coverPreviewImg');
    var hero = document.getElementById('wizardPreviewHero');
    if (!file || !img) return;

    var reader = new FileReader();
    reader.onload = function (e) {
        img.src = e.target.result;
        img.classList.add('has-image');

        if (hero) {
            hero.style.backgroundImage = 'url(' + e.target.result + ')';
            hero.classList.add('has-cover');
        }
    };
    reader.readAsDataURL(file);
}

function generateDescription() {
    var titleInput = document.getElementById('titleInput');
    var descriptionInput = document.getElementById('descriptionInput');
    var btn = document.getElementById('generateDescriptionBtn');
    var eventTypeRadio = document.querySelector('input[data-event-type-radio]:checked');

    if (!descriptionInput || !btn) return;

    var title = titleInput ? titleInput.value.trim() : '';
    var eventType = eventTypeRadio ? eventTypeRadio.value : 0;

    var originalHtml = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<i class="bi bi-hourglass-split"></i> Generating...';

    fetch('/Events/GenerateDescription?title=' + encodeURIComponent(title) + '&eventType=' + encodeURIComponent(eventType))
        .then(function (r) { return r.json(); })
        .then(function (result) {
            if (result && result.description) {
                descriptionInput.value = result.description;
                updateCharCount('descriptionInput', 'descriptionCount', 500);
            } else {
                alert('Could not generate a description right now — try again in a moment.');
            }
        })
        .catch(function () {
            alert('Could not generate a description right now — try again in a moment.');
        })
        .finally(function () {
            btn.disabled = false;
            btn.innerHTML = originalHtml;
        });
}

function selectSearchedPlace(id, name, city, country) {
    var hidden = document.getElementById('selectedPlaceIdInput');
    var venueInput = document.getElementById('venueNameInput');
    var cityInput = document.getElementById('cityInput');
    var countryInput = document.getElementById('countryInput');
    var resultsBox = document.getElementById('locationSearchResults');

    if (hidden) hidden.value = id;
    if (venueInput) venueInput.value = name;
    if (cityInput && city) cityInput.value = city;
    if (countryInput && country) countryInput.value = country;
    if (resultsBox) resultsBox.innerHTML = '';
}

document.addEventListener('DOMContentLoaded', function () {
    var searchInput = document.getElementById('locationSearchInput');

    if (searchInput) {
        var timeoutId = null;

        searchInput.addEventListener('input', function () {
            clearTimeout(timeoutId);
            var query = searchInput.value.trim();
            var resultsBox = document.getElementById('locationSearchResults');

            if (!resultsBox) return;

            if (query.length < 2) {
                resultsBox.innerHTML = '';
                return;
            }

            timeoutId = setTimeout(function () {
                fetch('/Events/SearchPlaces?query=' + encodeURIComponent(query))
                    .then(function (r) { return r.json(); })
                    .then(function (places) {
                        resultsBox.innerHTML = '';

                        if (!places.length) {
                            resultsBox.innerHTML = '<div class="location-search-result">No matches found.</div>';
                            return;
                        }

                        places.forEach(function (place) {
                            var row = document.createElement('div');
                            row.className = 'location-search-result';
                            row.textContent = place.name + (place.city ? (', ' + place.city) : '') + (place.country ? (', ' + place.country) : '');
                            row.onclick = function () {
                                selectSearchedPlace(place.id, place.name, place.city, place.country);
                            };
                            resultsBox.appendChild(row);
                        });
                    })
                    .catch(function () { /* best-effort search — ignore network errors */ });
            }, 300);
        });
    }

    var allDayToggle = document.getElementById('allDayToggle');
    if (allDayToggle) {
        var toggleTimeFields = function () {
            var timeFields = document.querySelectorAll('.time-field');
            timeFields.forEach(function (el) {
                el.style.display = allDayToggle.checked ? 'none' : '';
            });
        };
        allDayToggle.addEventListener('change', toggleTimeFields);
        toggleTimeFields();
    }
});
