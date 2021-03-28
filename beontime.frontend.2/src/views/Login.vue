<template>
  <div class="login__container">
    <h3 class="login__title">Login</h3>

    <form @submit.prevent="loginUser">
      <div class="login">
        <input v-model="email" type="email" placeholder="login" />
      </div>
      <div class="password">
        <input v-model="password" type="password" placeholder="password" />
      </div>
      <button type="submit" class="login__button">Login</button>
    </form>

    <div v-if="error" class="error">{{ error.message }}</div>

    <p>
      Need an account? Click here to
      <router-link :to="{ name: 'signup' }">Register</router-link>
    </p>
  </div>
</template>

<script lang="ts">
import { Vue, setup } from "vue-class-component";
import { useVuelidate } from "@vuelidate/core";
import firebase from "firebase/app";

export default class Login extends Vue {
  emailAddress = "";
  password = "";

  requestError = "";

  $v = setup(() => useVuelidate());

  async login(): Promise<void> {
    this.requestError = "";
    this.$v.$touch();

    if (this.$v.$invalid) return;

    try {
      await firebase
        .auth()
        .signInWithEmailAndPassword(this.emailAddress, this.password);

      this.$router.push({ name: "Reports" });
    } catch (er) {
      if (er.code == "auth/user-not-found") {
        this.requestError = "Użytkownik nie istnieje!";
      } else if (er.code == "auth/wrong-password") {
        this.requestError = "Niepoprawne hasło!";
      } else {
        this.requestError = er.message;
        console.error({ error: er });
      }
    }
  }
}
</script>

<style lang="less" scoped>
.login {
  &__container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-items: center;
    min-height: 100vh;
  }

  &__title {
    @apply from-red-800;
  }

  &__button {
    @apply relative;
    @apply w-full;
    @apply flex;
    @apply justify-center;
    @apply py-2;
    @apply px-4;
    @apply border;
    @apply border-transparent;
    @apply text-sm;
    @apply font-medium;
    @apply rounded-md;
    @apply text-white;
    @apply bg-indigo-600;

    &:hover {
      @apply bg-indigo-700;
    }

    &:focus {
      @apply outline-none;
    }
  }
}
</style>
