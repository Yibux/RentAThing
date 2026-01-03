window.firebaseAuth = {
    register: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return token;
        } catch (err) {
            console.error("Firebase register error:", err);
            throw err;
        }
    }
};
