<template>
    <component
        :is="componentTag"
        class="be-button"
        :to="to"
        :href="href"
        :color="color"
        :disabled="disabled"
    >
        <font-awesome-icon
            v-if="icon"
            :icon="icon"
            class="be-button__icon"
        />
        <span
            class="be-button__value"
        >
            <slot />
        </span>
    </component>
</template>

<script lang='ts'>
import { defineComponent, PropType, computed, ref } from "vue";

interface Tags {
    link: () => string;
    anchor: () => string;
    button: () => string;
}

export default defineComponent({
    name: "BeButton",
    props: {
        href: {
            type: String,
            default: null
        },
        to: {
            type: String,
            default: null
        },
        disabled: {
            type: Boolean,
            default: false
        },
        type: {
            type: String as PropType<string>,
            default: "button"
        },
        color: {
            type: String,
            default: "default"
        },
        icon: {
            type: String,
            default: ""
        }
    },
    setup(props) {
        const loading = ref(false);

        const componentTag = computed((): string => {
            const tags = {
                link: () => "router-link",
                anchor: () => "a",
                button: () => "button",
            } as Tags;

            return tags[props.type as keyof Tags]();
        });

        return { loading, componentTag };
    },
});
</script>

<style lang='less' scoped>
@default-color: #9f9f9f;
@default-hover-color: #2c3e50;

@primary-color: #22a7f0;
@primary-hover-color: #0d7cb9;

@success-color: #019875;
@success-hover-color: #01654e;

@warning-color: #f4b350;
@warning-hover-color: #e9920f;

@error-color: #d91e18;
@error-hover-color: #941410;

@disabled-color: #dadada;

.button-color(@color, @hover-color) {
    @apply "border-@{color}-600";
    @apply "bg-@{color}-600";
    @apply "text-white";

    &:hover {
        @apply "border-@{color}-700";
        @apply "bg-@{color}-700";
    }
}

.be-button {
    cursor: pointer;
    border: none;
    background: 0 0;
    @apply text-base;
    @apply rounded-md;
    text-decoration: none;

    padding: 6px 24px;
    outline: 0;

    user-select: none;
    display: flex;
    justify-content: center;
    align-items: center;

    @apply transition;
    @apply duration-200;

    &::before, &::after {
        content: '';
        flex: 1 0 auto;
    }

    &:disabled {
        cursor: not-allowed;
        opacity: 0.333;
    }

    &[color="default"] {
        .button-color(@default-color, @default-hover-color);
    }

    &[color="primary"] {
        .button-color(@primary-color, @primary-hover-color);
    }

    &[color="success"] {
        .button-color(@success-color, @success-hover-color);
    }

    &[color="warning"] {
        .button-color(@warning-color, @warning-hover-color);
    }

    &[color="danger"] {
        .button-color(@error-color, @error-hover-color);
    }
}
</style>
