const angle = 25;
const smiley = document.getElementById("smiley");
const loginCard = document.getElementById("login_card");

loginCard.addEventListener("mousemove", (evt) => {
	const rect = loginCard.getBoundingClientRect();
	const { x: posX, y: posY, width, height } = rect;
	const { clientX, clientY } = evt;
	const tmpX = clientX - posX - width / 2;
	const tmpY = clientY - posY - height / 2;
	const x = Math.round(tmpX / (width / 2) * angle * 10000) / 10000;
	const y = Math.round(tmpY / (height / 2) * angle * -1 * 10000) / 10000;
	smiley.style.setProperty("--rotate-x", y + "deg");
	smiley.style.setProperty("--rotate-y", x + "deg");
});

loginCard.addEventListener("mouseout", (evt) => {
	smiley.style.setProperty("--rotate-x", "0deg");
	smiley.style.setProperty("--rotate-y", "0deg");
});
