(function (global) {
    const exports = {
         version: "1.0.0"
        ,async init() {
            var _msgboxHtml = await su.getHtmlTemplate("p/templates/msg-toast.html");

            if($("#msgbox").length == 0){
                $("#app").append(_msgboxHtml);
                su.msgBox.toastTitle =  document.getElementById("toastTitle");
                su.msgBox.toastMessage = document.getElementById("toastMessage");
                su.msgBox.msgToast = document.getElementById("msgToast");

            }        
   
        }
        ,async show(title, body) {
           await this.init();
           su.msgBox.setText(title,body);
           let toast = new bootstrap.Toast(su.msgBox.msgToast);
           toast.show();
        }
        ,async success(body) {
           await this.init();
           su.msgBox.setText("Success",body);
           su.msgBox.toastMessage.innerHTML = body;
            let toast = new bootstrap.Toast(su.msgBox.msgToast);
            toast.show();
        }

        ,async error(body) {
           await this.init();
           su.msgBox.setText("Error",body);
           su.msgBox.toastMessage.innerHTML = body;
           let toast = new bootstrap.Toast(su.msgBox.msgToast);
           toast.show();
        }

        ,async info(body) {
           await this.init();
           su.msgBox.setText("Infomation",body);
           su.msgBox.toastMessage.innerHTML = body;
           let toast = new bootstrap.Toast(su.msgBox.msgToast);
           toast.show();
        }

        , setText(title, body){
            su.msgBox.toastTitle.innerHTML = title;
            su.msgBox.toastMessage.innerHTML = body;
        }

        
    }
    if (!global) throw new Error("su-core.js is required.");
    global.msgToast =  exports;     

})(su);