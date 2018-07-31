window.upload = {
    uploadFile: function () {
        var input = document.getElementById('file-upload');
        var file = input.files[0];
        var reader = new FileReader();

        return new Promise((resolve, reject) => {
            if (!file) {
                resolve("");
            }
            else if (file.size > 3 * 1000 * 1024) {
                input.value = null;
                resolve("Error: File too large (3MB limit)");
            }
            else {
                reader.onload = (e) => {
                    resolve(e.target.result);
                };
                reader.readAsDataURL(file);
            }
        });
    }
};