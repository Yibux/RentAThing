window.firebaseAuth = {
    register: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return { success: true, token: token, error: null };
        } catch (err) {
            return { success: false, token: null, error: err.code || "unknown_error" };
        }
    },

    login: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return { success: true, token: token, error: null };
        } catch (err) {
            return { success: false, token: null, error: err.code || "unknown_error" };
        }
    },

    passwordReset: async function (email) {
        try {
            if (!firebase.apps.length) firebase.initializeApp(window.firebaseConfig);
            await firebase.auth().sendPasswordResetEmail(email);
            return { success: true, error: null };
        } catch (err) {
            return { success: false, error: err.code || "unknown_error" };
        }
    }
};