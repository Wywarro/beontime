<template>
  <div>
    <header>
      <Topbar v-model:drawer-opened="drawerOpened" />
    </header>
    <NavigationDrawer v-model:drawer-opened="drawerOpened" />
    <main
      class="transform transition-all duration-300 pt-16 p-4"
      :class="{ 'ml-64': drawerOpened }"
    >
      <router-view />
    </main>
  </div>
</template>

<script lang="ts">
import { ref, onMounted, provide } from "vue";
import { Options, Vue, setup } from "vue-class-component";

import Topbar from "@/components/Topbar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";

import firebase from "firebase/app";
import "firebase/auth";

import userService from "@/services/UserService";
import dateService from "@/services/DateService22";

@Options({
  components: {
    Topbar,
    NavigationDrawer
  }
})
export default class App extends Vue {
  composition = setup(() => {
    provide("userService", userService);
    provide("dateService", dateService);
  });

  drawerOpened = true;

  mounted() {
    firebase.auth().onAuthStateChanged((user: firebase.User | null) => {
      userService.fetchUser(user);
    });
  }
}
</script>

<style lang="less">
body {
  font-family: "Lato", sans-serif;
}

* {
  box-sizing: border-box;
}
</style>
