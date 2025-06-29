const dropArea = document.getElementById('drop-area');
const fileInput = document.getElementById('evidencia-derivacion-id-input');
const modalGeneral = document.getElementById('evidencia-derivacion-modal');
const modalReagendamiento = document.getElementById('GeneralTemplateModal');
// const fileList = document.getElementById('file-list');

let files = [];
var activeId = null;


async function modal_id_derivacion_to_be_uploaded(id) {
    activeId = id;
    const modalTitle = document.getElementById('evidencia-derivacion-title');
    modalTitle.textContent = `Evidencia para la derivación: ${activeId}`;
    fileInput.value = ''; // Limpiar el input file
    files = []; // Limpiar la lista de archivos
    updateFileList(); // Actualizar la lista visualmente
}

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

// Pegar (Ctrl+V) en el modal de reagendamiento
modalReagendamiento.addEventListener('paste', (e) => {
    const items = e.clipboardData.items;
    for (let item of items) {
        if (item.kind === 'file') {
            const file = item.getAsFile();
            files.push(file);
            updateFileInput();
            updateFileList('reagendamiento');
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
function updateFileList(type = 'evidencia') {
    let fileListActive;
    if (type === 'reagendamiento') {
        fileListActive = document.getElementById('file-list-reagendamiento');
    } else {
        fileListActive = document.getElementById('file-list');
    }
    fileListActive.innerHTML = '';
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
        fileListActive.appendChild(listItem);
    });
}

// Actualizar el input[type=file] para que contenga los archivos en `files[]`
function updateFileInput() {
    const dt = new DataTransfer();
    files.forEach(f => dt.items.add(f));
    fileInput.files = dt.files;
}

async function submit_evidencia_derivacion() {
    const formData = [];
    const dataJson = [];
    for (const file of files) {
        const hex = await archivoAHex(file);
        formData.push(hex);
        dataJson.push({
            fileName: file.name,
            fileType: file.name.split('.').pop(), // Obtener la extensión del archivo
            fileContent: hex,
            idDerivacion: activeId,
            type: 0 // Asumiendo que el tipo es 0 para evidencia
        });
    }

    if (formData.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'No hay archivos',
            text: 'Por favor, arrastra o selecciona archivos para subir.',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const convertedFiles = JSON.stringify(dataJson);
    console.log('Archivos a subir:', convertedFiles);

    const loading = Swal.fire({
        title: 'Subiendo archivos...',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    try {
        const response = await fetch('/Derivacion/UploadEvidencia', {
            method: 'POST',
            body: convertedFiles,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        Swal.close();

        if (!response.ok) {
            Swal.fire({
                icon: 'error',
                title: 'Error al subir archivos',
                text: 'Por favor, intenta de nuevo más tarde.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        const result = await response.json();
        if (result.success) {
            Swal.fire({
                icon: 'success',
                title: 'Archivos subidos',
                text: 'Los archivos se han subido correctamente.',
                confirmButtonText: 'Aceptar'
            });
            files = []; // Limpiar la lista de archivos
            updateFileList(); // Actualizar la lista visualmente
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.message || 'Ocurrió un error al subir los archivos.',
                confirmButtonText: 'Aceptar'
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error de conexión',
            text: 'No se pudo conectar al servidor. Por favor, verifica tu conexión a Internet.',
            confirmButtonText: 'Aceptar'
        });
    }
}

async function archivoAHex(file) {
    const buffer = await file.arrayBuffer();
    return '0x' + [...new Uint8Array(buffer)]
        .map(b => b.toString(16).padStart(2, '0'))
        .join('');
}
