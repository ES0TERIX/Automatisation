import express from "express";
import multer from "multer";
import morgan from "morgan";
import fs from "fs";
import cors from "cors";

const app = express();
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(morgan("combined"));
app.use(
	cors({
		credentials: true,
		origin: "http://localhost:4000",
	}),
);

const port = 3000;

const storage = multer.memoryStorage(); // Store files in memory
const upload = multer({ storage: storage });

app.post("/sauvegarder", upload.single("file"), (req, res) => {
	const fileBinary = req.file;
	fs.writeFile(
		"/etc/data/" + req.file.originalname,
		fileBinary.buffer,
		(err) => {
			if (err) {
				console.error(err);
				return;
			}
		},
	);

	res.status(200).send(fileBinary);
});

app.get("/download", (req, res) => {
	const file = "/etc/data/test.csv";
	res.download(file);
});

app.get("/calculmoyenne", async (req, res) => {
    try {
        const calcul = await fetch("http://serviceio:80/calculmoyenne", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        });

        // Log information about the fetch request
        console.log(`Fetch to http://serviceio:80/calculmoyenne: Status ${calcul.status}`);

        const responseText = await calcul.text();
        console.log("Full Response Body:", responseText);

	

        if (responseText) {
            const result = JSON.parse(responseText);

            res.status(200).json(result);
        } else {
            res.status(500).json({ error: "Unexpected response format" });
        }
    } catch (err) {
        console.error("Error during fetch:", err);
        res.status(500).json({ error: "Internal Server Error" });
    }
});


app.listen(port, () => {
	console.log(`Example app listening at http://localhost:${port}`);
});
