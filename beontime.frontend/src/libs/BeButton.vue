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
            default: "primary"
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

    &[color="primary"] {
        @apply bg-blue-600;
        @apply text-white;

        &:hover {
            @apply bg-blue-700;
        }
    }

    &[color="secondary"] {
        @apply bg-indigo-600;
        @apply text-white;

        &:hover {
            @apply bg-indigo-700;
        }
    }

    &[color="success"] {
        @apply bg-green-600;
        @apply text-white;

        &:hover {
            @apply bg-green-700;
        }
    }

    &[color="warning"] {
        @apply bg-orange-600;
        @apply text-white;

        &:hover {
            @apply bg-orange-700;
        }
    }

    &[color="danger"] {
        @apply bg-red-600;
        @apply text-white;

        &:hover {
            @apply bg-red-700;
        }
    }
}
</style>
