const submit = document.getElementById("submit");
const form = document.getElementById("contact-form");
const formState = { token: "", email: "", message: "" };

function onRecaptchaSubmit(token) {
    formState.token = token;
    submit.removeAttribute("disabled");
}

function onRecaptchaExpired() {
    formState.token = "";
    submit.setAttribute("disabled", "disabled");
}

function readFormDataIntoState() {
    const emailInput = document.getElementById("email-address");
    formState.email = emailInput.value;

    const messageInput = document.getElementById("message");
    formState.message = messageInput.value;
}

function showThanksMessage() {
    const form = document.querySelector("form");
    form.innerHTML = `<div class="thanks"><h2>Thanks for Contacting Us!</h2><p>We will reply as soon as possible.</p></div>`;
}

form.onsubmit = async function (event) {
    event.preventDefault();
    readFormDataIntoState();

    const response = await fetch("https://commands.brigh.id/commands/email/execute/recaptcha", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "G-Recaptcha-Response": formState.token,
        },
        body: JSON.stringify({
            To: "recitalblooms@gmail.com",
            From: formState.email,
            Subject: "Recital Blooms Contact Form Submission",
            Message: formState.message,
        })
    });

    if (response.status === 200) {
        showThanksMessage();
    }
}

// showThanksMessage();