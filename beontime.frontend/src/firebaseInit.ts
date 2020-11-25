import firebase from "firebase/app";
import "firebase/analytics";

import firebaseConfig from "./firebaseConfig";

firebase.initializeApp(firebaseConfig);
firebase.analytics();
