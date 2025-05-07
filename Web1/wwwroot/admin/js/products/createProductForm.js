document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('createProductForm');

    form.addEventListener('submit', () => {
        savePhotos();
    });

    tinymce.init({
        selector: '#description',
        plugins: 'advlist autolink link image lists charmap preview anchor pagebreak searchreplace wordcount code fullscreen insertdatetime media table help',
        toolbar: 'undo redo | styles | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | preview fullscreen',
        menubar: 'file edit view insert format tools table help'
    });

    new Sortable(document.getElementById('imageList'), {
        animation: 150,
        ghostClass: 'opacity-50'
    });

    const fileInput = document.getElementById('fileInput');
    const imageList = document.getElementById('imageList');

    fileInput.addEventListener('change', handleFiles);

    let imageCounter = 0;

    function handleFiles(event) {
        const files = event.target.files;

        Array.from(files).forEach(file => {
            if (!file.type.startsWith('image/')) return;

            const reader = new FileReader();
            reader.onload = function (e) {
                const imgId = `image-${imageCounter++}`;

                const imageHTML = `
                <div class="position-relative" id="${imgId}" style="display: inline-block; max-width: 200px;">
                    <img src="${e.target.result}" 
                         class="img-fluid rounded border" 
                         style="max-height: 150px; object-fit: cover;" 
                         alt="Preview" />
                    <button type="button" 
                            class="btn btn-sm position-absolute top-0 end-0 m-1 rounded-circle"
                            style="background-color: rgba(220, 53, 69, 0.5); font-size: 30px; max-width: 5px;  max-height: 30px"
                            onclick="document.getElementById('${imgId}').remove()" >
                        &times;
                    </button>
                </div>
            `;

                imageList.innerHTML += imageHTML;
            };

            reader.readAsDataURL(file);
        });

        fileInput.value = '';
    }

    function savePhotos() {
        form.querySelectorAll('input[name^="Images"]').forEach(el => el.remove());

        const imgs = imageList.querySelectorAll('img');

        imgs.forEach((img, index) => {
            const hiddenName = document.createElement('input');
            hiddenName.type = 'hidden';
            hiddenName.name = `Images[${index}].Name`;
            hiddenName.value = img.src;
            form.appendChild(hiddenName);

            const hiddenPriority = document.createElement('input');
            hiddenPriority.type = 'hidden';
            hiddenPriority.name = `Images[${index}].Priority`;
            hiddenPriority.value = index;
            form.appendChild(hiddenPriority);
        });
    }
});