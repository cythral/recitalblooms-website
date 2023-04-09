
const slides = document.querySelector("#slides ul");
slides.style.left = "0";

function animate_slides(done) {
    const interval = setInterval(function() {
        const current = parseFloat(slides.style.left);
        if (current === -100) {
            clearInterval(interval);
            done();
            return;
        }
        
        slides.style.left = `${(current - 1)}%`;
    }, 5);
}

setInterval(function() {
   animate_slides(function() {
    const firstChild = document.querySelector("#slides ul li:first-child");
    const parent = firstChild.parentElement;

    setTimeout(function() {
        parent.removeChild(firstChild);
        parent.style.left = "0";
        parent.appendChild(firstChild);
    }, 1000);
   });
}, 5000);