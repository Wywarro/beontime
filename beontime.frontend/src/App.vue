<template>
  <Topbar />
  <NavigationDrawer :is-open="drawerOpened" />
  <router-view/>
  <font-awesome-icon type="fas" icon="bars" />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Topbar from "@/components/Topbar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";

export default defineComponent({
  name: "Home",
  components: {
    Topbar,
    NavigationDrawer,
  },
  data: () => ({
    drawerOpened: false,
  }),
});
</script>

<style lang="less">

</style>
