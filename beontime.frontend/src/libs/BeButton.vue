<template>
    <button>
        <slot />
    </button>
</template>

<script lang='ts'>
import { defineComponent } from "vue";

export default defineComponent({
    name: "BeButton",
});
</script>

<style lang='less' scoped>

</style>
