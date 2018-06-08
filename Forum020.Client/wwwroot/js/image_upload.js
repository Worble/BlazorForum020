Blazor.registerFunction('readFile', () => {
    var preview = document.getElementById('image');
    var input = document.getElementById('image-input');
    var file = document.getElementById('file-upload').files[0];
    var button = document.getElementById('submit');
    var reader = new FileReader();

    return new Promise((resolve, reject) => {
        if (!file) {
            resolve("");
        }
        else if (file.size > 3 * 1000 * 1024) {
            resolve("Error: File too large (3MB limit)");
        }
        else {        
            reader.onload = (e) => {
                resolve(e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});