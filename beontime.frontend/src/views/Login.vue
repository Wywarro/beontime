<template>
    <div class="login__container">
        <h3>Login</h3>

        <form @submit.prevent="loginUser">
            <div class="login">
                <input
                    v-model="email"
                    type="email"
                    placeholder="login"
                />
            </div>
            <div class="password">
                <input
                    v-model="password"
                    type="password"
                    placeholder="password"
                />
            </div>
            <button type="submit">Login</button>
        </form>

        <div
            v-if="error"
            class="error"
        >{{ error.message }}</div>

        <p>
            Need an account? Click here to
            <router-link :to="{ name: 'signup' }">Register</router-link>
        </p>
    </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRouter } from "vue-router";

import firebase from "firebase";

export default defineComponent({
    name: "Login",
    setup() {
        const email = ref("");
        const password = ref("");
        const error = ref("");

        const router = useRouter();
        const loginUser = async(): Promise<void> => {
            try {
                const user = await firebase
                    .auth()
                    .signInWithEmailAndPassword(email.value, password.value);

                console.log(user);
                router.replace({ name: "home" });
            } catch (error) {
                error.value = error;
            }
        };

        return { email, password, error, loginUser };
    }

});
</script>

<style lang="less" scoped>
.login {
    &__container {
        display: flex;
    }
}
</style>
