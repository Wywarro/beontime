import { shallowMount, VueWrapper } from "@vue/test-utils";

import WorkCalendar from "@/views/WorkCalendar.vue";
import FontAwesomeIcon from "@/libs/FontAwesomeIcon.vue";
import BeButton from "@/libs/BeButton.vue";

import { User, UserService } from "@/services/userService";
import { pl } from "date-fns/locale";
import { Component } from "vue";

describe("WorkCalendar.vue", () => {
    let userService: UserService;
    let components: Record<string, Component>;
    beforeAll(() => {
        userService = {
            user: {
                data: {
                    displayName: "pozdro@z.gory"
                },
                preferences: {
                    locale: pl
                }
            } as User,
            fetchUser: jest.fn()
        };

        components = { FontAwesomeIcon, BeButton };
    });
    describe("general behaviour", () => {
        let wrapper: VueWrapper<any>;
        beforeEach(() => {
            wrapper = shallowMount(WorkCalendar, {
                global: {
                    provide: {
                        userService,
                        dateService: {
                            getNow: () => new Date(2020, 9, 14, 22, 5, 30)
                        }
                    },
                    components
                }
            });
        });

        afterEach(() => {
            wrapper.unmount();
        });

        it("renders 23 divs with .cal__row", () => {
            const rows = wrapper.findAll(".cal__row");
            expect(rows.length).toBe(23);
        });

        it("renders 23 divs with .cal__time", () => {
            const rows = wrapper.findAll(".cal__time");
            expect(rows.length).toBe(23);
        });

        it("renders 7 divs with .cal__col", () => {
            const rows = wrapper.findAll(".cal__col");
            expect(rows.length).toBe(7);
        });

        it("renders 12. października till 18. października", () => {
            const allDayColumns = wrapper.findAll(".cal__day");
            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 12}. października 2020`);
            });
        });

        it("renders 5. października till 11. października when clicked user substracted 1 week", async() => {
            const substractWeekButton = wrapper.find("[data-cy=substractWeekButton]");
            await substractWeekButton.trigger("click");

            const allDayColumns = wrapper.findAll(".cal__day");
            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 5}. października 2020`);
            });
        });

        it("renders 19. października till 25. października when clicked user added 1 week", async() => {
            const addWeekButton = wrapper.find("[data-cy=addWeekButton]");
            await addWeekButton.trigger("click");

            const allDayColumns = wrapper.findAll(".cal__day");
            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 19}. października 2020`);
            });
        });

        it("does not render current time indicator if user added one week", async() => {
            const substractWeekButton = wrapper.find("[data-cy=substractWeekButton]");
            await substractWeekButton.trigger("click");

            const currentTimeIndicator = wrapper.find(".cal__current-time");
            expect(currentTimeIndicator.exists()).toBeFalsy();
        });

        it("renders current time indicator if current week", () => {
            const currentTimeIndicator = wrapper.find(".cal__current-time");
            expect(currentTimeIndicator.exists()).toBeTruthy();
        });

        it("does not render current time indicator if user substracted one week", async() => {
            const addWeekButton = wrapper.find("[data-cy=addWeekButton]");
            await addWeekButton.trigger("click");

            const currentTimeIndicator = wrapper.find(".cal__current-time");
            expect(currentTimeIndicator.exists()).toBeFalsy();
        });

        it("renders current-time indicator when user clicks on todays button", async() => {
            const substractWeekButton = wrapper.find("[data-cy=substractWeekButton]");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");

            const allDayColumns = wrapper.findAll(".cal__day");
            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 14}. września 2020`);
            });

            const todayButton = wrapper.find("[data-cy=todayButton]");
            await todayButton.trigger("click");

            const currentTimeIndicator = wrapper.find(".cal__current-time");
            expect(currentTimeIndicator.exists()).toBeTruthy();
        });

        it("renders 12. października till 18. października when user clicks on todays button", async() => {
            const substractWeekButton = wrapper.find("[data-cy=substractWeekButton]");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");
            await substractWeekButton.trigger("click");

            const allDayColumns = wrapper.findAll(".cal__day");
            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 14}. września 2020`);
            });

            const todayButton = wrapper.find("[data-cy=todayButton]");
            await todayButton.trigger("click");

            allDayColumns.map((dayCol, index) => {
                expect(dayCol.text()).toContain(`${index + 12}. października 2020`);
            });
        });
    });

    describe("current time indicator", () => {
        it("renders indicator on 5th column 23th row and 8% top properties", () => {
            const wrapper = shallowMount(WorkCalendar, {
                global: {
                    provide: {
                        userService,
                        dateService: {
                            getNow: () => new Date(2020, 9, 14, 22, 5, 30)
                        }
                    },
                    components
                }
            });
            const currentTimeIndicator = wrapper.find(".cal__current-time");
            const indicatorStyles = currentTimeIndicator.attributes().style;
            expect(indicatorStyles).toBe("grid-column: 5; grid-row: 23; top: 8%;");
        });

        it("renders indicator on 7th column 16th row and 37% top properties", () => {
            const wrapper = shallowMount(WorkCalendar, {
                global: {
                    provide: {
                        userService,
                        dateService: {
                            getNow: () => new Date(2020, 7, 21, 15, 22, 16)
                        }
                    },
                    components
                }
            });
            const currentTimeIndicator = wrapper.find(".cal__current-time");
            const indicatorStyles = currentTimeIndicator.attributes().style;
            expect(indicatorStyles).toBe("grid-column: 7; grid-row: 16; top: 37%;");
        });

        it("renders indicator on 4th column 4th row and 18% top properties", () => {
            const wrapper = shallowMount(WorkCalendar, {
                global: {
                    provide: {
                        userService,
                        dateService: {
                            getNow: () => new Date(2020, 2, 10, 3, 11, 59)
                        }
                    },
                    components
                }
            });
            const currentTimeIndicator = wrapper.find(".cal__current-time");
            const indicatorStyles = currentTimeIndicator.attributes().style;
            expect(indicatorStyles).toBe("grid-column: 4; grid-row: 4; top: 18%;");
        });

        it("renders indicator on 6th column 1st row and 0% top properties", () => {
            const wrapper = shallowMount(WorkCalendar, {
                global: {
                    provide: {
                        userService,
                        dateService: {
                            getNow: () => new Date(2020, 11, 31, 0, 0, 0)
                        }
                    },
                    components
                }
            });
            const currentTimeIndicator = wrapper.find(".cal__current-time");
            const indicatorStyles = currentTimeIndicator.attributes().style;
            expect(indicatorStyles).toBe("grid-column: 6; grid-row: 1; top: 0%;");
        });
    });
});
