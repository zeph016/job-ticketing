
const list = [];
var maxNumber = 0;

function test() {
    generateRandomNumber();
    // document.getElementById("generateButton").onclick = function () {
    //     generateRandomNumber();
    // }
}

function generateRandomNumber() {
    maxNumber = parseFloat(document.getElementById("textbox").value);
    let randomNumber = Math.floor(Math.random() * maxNumber) + 1;
    if(list.includes(randomNumber)) {
        generateRandomNumber();
    } else {
        list.push(randomNumber);
        var displaylist = document.getElementById("random-textbox");
        displaylist.value = list.join("\n",);
    }
}

function DialogPopUp() {
    $('#inline-popups').magnificPopup({
        delegate: 'a',
        removalDelay: 500, //delay removal by X to allow out-animation
        callbacks: {
        beforeOpen: function() {
            this.st.mainClass = this.st.el.attr('data-effect');
        }
        },
        midClick: true // allow opening popup on middle mouse click. Always set it to true if you don't provide alternative source.
    });
}

function ShowDialog(elementId) {
    if (elementId != null && elementId != undefined) {
        let dialogElement = document.getElementById(elementId);
        dialogElement.classList.remove = "zoom-out";
        dialogElement.classList.add = "zoom-in";
        return "show dialog";
    } else {
        return "error";
    }
}

function CloseDialog(elementId) {
    if (elementId != null && elementId != undefined) {
        let dialogElement = document.getElementById(elementId);
        dialogElement.classList.remove = "zoom-in";
        dialogElement.classList.add = "zoom-out";
        return "close dialog";
    } else {
        return "error";
    }
}