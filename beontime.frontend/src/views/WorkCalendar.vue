<template>
    <div>
        <div class="cal__container">
            <div class="cal__header">
                <font-awesome-icon
                    icon="chevron-circle-left"
                    data-cy="substractWeekButton"
                    class="cal__nav-icon"
                    @click="subtractOneWeek"
                />

                <div class="cal__title">{{ formatDate(currentDate, monthFormatTokens) }}</div>

                <font-awesome-icon
                    icon="chevron-circle-right"
                    data-cy="addWeekButton"
                    class="cal__nav-icon"
                    @click="addOneWeek"
                />
            </div>
            <div class="cal__days">
                <div />
                <div />
                <div
                    v-for="day in days"
                    :key="`30${day}`"
                    class="cal__day"
                >{{ formatDate(getDayFromMonday(day), dayFormatTokens) }}</div>
            </div>
            <div class="cal__content">
                <div
                    v-for="hour in hours"
                    :key="hour"
                    class="cal__time"
                    :style="{ 'grid-row': hour }"
                >
                    {{ hourTime(hour) }}
                </div>
                <div class="cal__filler-col" />
                <div
                    v-for="day in days"
                    :key="`10${day}`"
                    class="cal__col"
                    :style="{ 'grid-column': day + 3 }"
                />
                <div
                    v-for="hour in hours"
                    :key="`20${hour}`"
                    class="cal__row"
                    :style="{ 'grid-row': hour }"
                />
                <div
                    class="cal__current-time"
                    :style="{ 'grid-column': currentDayOnGrid, 'grid-row': currentHourOnGrid, 'top': currentMinute}"
                >
                    <div class="cal__circle" />
                </div>
            </div>
        </div>
        <CalendarNavigator class="cal__navigator" :month="currentMonth" />
    </div>
</template>

<script lang="ts">
import { defineComponent, ref, Ref, computed, onMounted, inject } from "vue";

import { enUS } from "date-fns/locale";
import {
    getHours,
    getDay,
    getMinutes,
    getMonth,

    addDays,
    addWeeks,
    addSeconds,

    startOfWeek,
    format,
    Locale
} from "date-fns";

import { range } from "lodash";

import CalendarNavigator from "@/components/CalendarNavigator.vue";
import { UserService } from "@/services/userService";
import { DateService } from "@/services/dateService";

export default defineComponent({
    name: "WorkCalendar",
    components: {
        CalendarNavigator
    },
    setup() {
        const dayFormatTokens = "do. MMMM yyyy";
        const monthFormatTokens = "wo. 'tydzień' MMMM yyyy";

        const hours = ref(range(1, 24));
        const days = ref(range(0, 7));

        const hourTime = (number: number) => {
            if (number < 10) {
                return `0${number}:00`;
            }
            return `${number}:00`;
        };

        const dateService = inject<DateService>("dateService") ?? { getNow: () => new Date() };
        const currentDate = ref(dateService.getNow()) as Ref<Date>;

        onMounted(() => {
            setInterval(() => {
                console.log({ currentDate: currentDate.value });
                currentDate.value = addSeconds(currentDate.value, 1);
            }, 1000);
        });

        const addOneWeek = () => {
            currentDate.value = addWeeks(currentDate.value, 1);
        };
        const subtractOneWeek = () => {
            currentDate.value = addWeeks(currentDate.value, -1);
        };

        const userService = inject<UserService>("userService");
        const locale: Locale = userService?.user?.preferences?.locale ?? enUS;

        const getDayFromMonday = (dayFromMonday: number) => {
            const monday = startOfWeek(currentDate.value, { locale });
            return addDays(monday, dayFromMonday);
        };

        const formatDate = (date: Date, formatTokens: string) => {
            return format(date, formatTokens, { locale });
        };

        const currentDayOnGrid = computed(() => {
            const gridAdjustment = 2;
            return getDay(currentDate.value) + gridAdjustment;
        });

        const currentMonth = computed(() => {
            return getMonth(currentDate.value);
        });

        const currentHourOnGrid = computed(() => {
            const gridAdjustment = 1;
            return getHours(currentDate.value) + gridAdjustment;
        });

        const currentMinute = computed(() => {
            const oneHourInMinutes = 60;
            const minutesPercentage = getMinutes(currentDate.value) / oneHourInMinutes * 100;

            return `${Math.round(minutesPercentage)}%`;
        });

        return {
            dayFormatTokens,
            monthFormatTokens,

            hours,
            days,
            hourTime,

            currentDate,

            addOneWeek,
            subtractOneWeek,

            getDayFromMonday,
            formatDate,

            currentDayOnGrid,
            currentMonth,
            currentHourOnGrid,
            currentMinute,

            userService
        };
    },
});
</script>

<style lang="less" scoped>
@title-height: 3em;
@days-height: 3em;
@time-width: 3em;
@time-height: 3em;
@grid-color: #dadce0;
@calendar-template: @time-width 10px repeat(7, 1fr);
@current-time-color: #ea4335;

.cal {
    &__container {
        width: 100%;
        display: grid;
        grid-template-rows: @title-height @days-height auto;
    }

    &__header {
        @apply bg-teal-600;
        text-align: center;
        display: flex;
        align-items: center;
        justify-content: space-between;
        z-index: 10;

        position: sticky;
        top: 4rem;
    }

    &__title {
        color: #fff;
    }

    &__nav-icon {
        color: #fff;
    }

    &__days {
        background: #f3f2f1;
        display: grid;
        place-content: center;
        text-align: center;
        grid-template-columns: @calendar-template;
        z-index: 10;
        border-bottom: 2px solid @grid-color;

        position: sticky;
        top: calc(4rem + @title-height);
    }

    &__day {
        border-left: 1px solid @grid-color;
    }

    &__content {
        display: grid;
        grid-template-columns: @calendar-template;
        grid-template-rows: repeat(24, @time-height);
    }

    &__time {
        grid-column: 1;
        text-align: right;
        align-self: end;
        font-size: 80%;
        position: relative;
        bottom: -0.5rem;
        color: #70757a;
        padding-right: 2px;
    }

    &__col {
        border-right: 1px solid @grid-color;
        grid-row: ~"1 / span 24";
        grid-column: span 1;
    }

    &__filler-col {
        grid-row: ~"1 / -1";
        grid-column: 2;
        border-right: 1px solid @grid-color;
    }

    &__row {
        grid-column: ~"2 / -1";
        border-bottom: 1px solid @grid-color;
    }

    &__current-time {
        border-top: 2px solid @current-time-color;
        position: relative;
    }

    &__circle {
        width: 12px;
        height: 12px;
        border: 1px solid @current-time-color;
        border-radius: 50%;
        background: @current-time-color;
        position: relative;
        top: -6px;
        left: -6px;
    }

    &__navigator {
        position: fixed;
        top: 500px;
        bottom: 33px;
        right: 20px;
    }

    &__nav-icon {
        @apply h-6;
        @apply w-1/4;
        @apply pr-5;
        display: inline-block;
    }
}
</style>
