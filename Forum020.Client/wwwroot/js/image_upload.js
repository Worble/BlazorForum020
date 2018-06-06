Blazor.registerFunction('readFile', () => {
    var preview = document.getElementById('image');
    var input = document.getElementById('image-input');
    var file = document.getElementById('file-upload').files[0];
    var button = document.getElementById('submit');
    var reader = new FileReader();

    function readFile(file) {
        return new Promise((resolve, reject) => {
            button.disabled = true;
            reader.onload = (e) => {
                resolve(e.target.result);
            };
            reader.readAsDataURL(file);
        })
    }

    if (file) {
        readFile(file).then(result => {
            preview.src = result;
            button.disabled = false;
        });
    };

    return true;
});

Blazor.registerFunction('readImageText', () => {
    var input = document.getElementById('image-input');
    var preview = document.getElementById('image');
    var file = document.getElementById('file-upload');

    var result = preview.src;

    preview.src = file.value = null;

    return result;
});