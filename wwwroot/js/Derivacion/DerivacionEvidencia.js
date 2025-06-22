const dropArea = document.getElementById('drop-area');
const fileInput = document.getElementById('evidencia-derivacion-id-input');
const modalBody = document.getElementById('evidencia-derivacion-modal-body');
const modalGeneral = document.getElementById('evidencia-derivacion-modal');
const fileList = document.getElementById('file-list');

let files = [];

// Click = abrir input oculto
dropArea.addEventListener('click', () => fileInput.click());

// Pegar (Ctrl+V)
modalGeneral.addEventListener('paste', (e) => {
    const items = e.clipboardData.items;
    for (let item of items) {
        if (item.kind === 'file') {
            const file = item.getAsFile();
            files.push(file);
            updateFileInput();
            updateFileList();
            break;
        }
    }
});

// Drag and Drop
dropArea.addEventListener('dragover', (e) => {
    e.preventDefault();
    dropArea.classList.add('bg-primary', 'text-white');
});

dropArea.addEventListener('dragleave', () => {
    dropArea.classList.remove('bg-primary', 'text-white');
});

dropArea.addEventListener('drop', (e) => {
    e.preventDefault();
    dropArea.classList.remove('bg-primary', 'text-white');
    for (let file of e.dataTransfer.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Cambio en input file
fileInput.addEventListener('change', () => {
    for (let file of fileInput.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Actualizar visualmente la lista de archivos
function updateFileList() {
    fileList.innerHTML = '';
    const filePreview = document.getElementById('file-preview');
    filePreview.classList.toggle('d-none', files.length === 0);

    files.forEach((file, index) => {
        const listItem = document.createElement('li');
        listItem.className = 'list-group-item d-flex justify-content-between align-items-center';

        const fileInfo = document.createElement('div');
        fileInfo.className = 'd-flex align-items-center';

        const fileName = document.createElement('span');
        fileName.textContent = file.name;

        fileInfo.appendChild(fileName);

        if (file.type.startsWith('image/')) {
            const img = document.createElement('img');
            img.className = 'img-thumbnail ms-3';
            img.style.maxHeight = '50px';
            img.style.maxWidth = '50px';

            const reader = new FileReader();
            reader.onload = ((imgElement) => {
                return (e) => {
                    imgElement.src = e.target.result;
                };
            })(img); // <- cerramos la función con `img` como argumento
            reader.readAsDataURL(file);

            fileInfo.appendChild(img);
        }

        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'btn btn-sm btn-danger';
        deleteBtn.innerHTML = '&times;';
        deleteBtn.title = 'Eliminar archivo';
        deleteBtn.onclick = () => {
            files.splice(index, 1);
            updateFileInput();
            updateFileList();
        };

        listItem.appendChild(fileInfo);
        listItem.appendChild(deleteBtn);
        fileList.appendChild(listItem);
    });
}

// Actualizar el input[type=file] para que contenga los archivos en `files[]`
function updateFileInput() {
    const dt = new DataTransfer();
    files.forEach(f => dt.items.add(f));
    fileInput.files = dt.files;
}
