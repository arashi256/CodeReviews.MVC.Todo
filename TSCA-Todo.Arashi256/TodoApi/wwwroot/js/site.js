const uri = '/todoitems';
let todos = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const item = {
        isComplete: false,
        name: addNameTextbox.value.trim()
    };
    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to add new item.");
            }
            return response.json();
        })
        .then(() => {
            getItems(); // Refresh the item list
            toastr.success("New item added successfully!", "Added");
            addNameTextbox.value = ''; // Clear the input field.
        })
        .catch(error => {
            console.error('Unable to add item.', error);
            toastr.error("Failed to add item.", "Error"); // Show error notification if add fails.
        });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Failed to delete item with ID ${id}`);
            }
            return response;
        })
        .then(() => {
            getItems(); // Refresh the item list after deletion.
            toastr.success("Item deleted successfully!", "Deleted");
        })
        .catch(error => {
            console.error('Unable to delete item.', error);
            toastr.error("Failed to delete item.", "Error"); // Show error notification if delete fails.
        });
}

function displayEditForm(id) {
    const item = todos.find(item => item.id === id);
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-isComplete').checked = item.isComplete;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        isComplete: document.getElementById('edit-isComplete').checked,
        name: document.getElementById('edit-name').value.trim()
    };
    fetch(`${uri}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Failed to update item with ID ${itemId}`);
            }
            return response;
        })
        .then(() => {
            getItems(); // Refresh the item list after update.
            toastr.success("Item updated successfully!", "Updated");
            closeInput(); // Hide the edit form.
        })
        .catch(error => {
            console.error('Unable to update item.', error);
            toastr.error("Failed to update item.", "Error"); // Show error notification if update fails.
        });
    return false;
}


function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'to-do' : 'to-dos';
    document.getElementById('counter').innerText = `There are ${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('todos');
    tBody.innerHTML = '';

    _displayCount(data.length);

    data.forEach(item => {
        let isCompleteCheckbox = document.createElement('input');
        isCompleteCheckbox.type = 'checkbox';
        isCompleteCheckbox.checked = item.isComplete;
        isCompleteCheckbox.disabled = true; // Keep it disabled (read-only).
        // Only apply Bootstrap class (CSS handles styling).
        isCompleteCheckbox.className = 'form-check-input';
        // Create "Edit" button with Bootstrap styles.
        let editButton = document.createElement('button');
        editButton.innerText = 'Edit';
        editButton.className = 'btn btn-primary btn-sm me-2'; // Bootstrap styles.
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);
        // Create "Delete" button with Bootstrap styles.
        let deleteButton = document.createElement('button');
        deleteButton.innerText = 'Delete';
        deleteButton.className = 'btn btn-danger btn-sm'; // Bootstrap styles.
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);
        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        td1.appendChild(isCompleteCheckbox);
        td1.className = "text-center";
        let td2 = tr.insertCell(1);
        let textNode = document.createTextNode(item.name);
        td2.appendChild(textNode);
        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);
        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });
    todos = data;
}

function resetAndCloseForm() {
    const form = document.querySelector("#editForm form");
    if (form)
    {
        form.reset(); // Reset form fields properly.
    }
    closeEditForm(); // Hide the form.
}

function closeEditForm() {
    document.getElementById("editForm").style.display = "none"; // Hide edit form.
}