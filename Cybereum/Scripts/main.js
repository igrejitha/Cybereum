(function () {
    // add month scale
    debugger;
    gantt.config.scale_unit = "week";
    gantt.config.step = 1;
    gantt.templates.date_scale = function (date) {
        var dateToStr = gantt.date.date_to_str("%d %M");
        var endDate = gantt.date.add(gantt.date.add(date, 1, "week"), -1, "day");
        return dateToStr(date) + " - " + dateToStr(endDate);
    };
    gantt.config.subscales = [
        { unit: "day", step: 1, date: "%D" }
    ];
    gantt.config.scale_height = 50;
    // configure milestone description
    gantt.templates.rightside_text = function (start, end, task) {
        if (task.type == gantt.config.types.milestone) {
            return task.text;
        }
        return "";
    };
    // add section to type selection: task, project or milestone
    gantt.config.lightbox.sections = [
        { name: "description", height: 70, map_to: "text", type: "textarea", focus: true },
        { name: "type", type: "typeselect", map_to: "type" },
        { name: "time", height: 72, type: "duration", map_to: "auto" }
    ];
    debugger;
    gantt.config.xml_date = "%Y-%m-%d %H:%i:%s"; // format of dates in XML
    //gantt.init("ganttContainer"); // initialize gantt
    //gantt.load("/Home/GetGanttData", "json");
    var tasks1 = {
        data: [
            {
                id: 1, text: "Project #1", start_date: "01-04-2013", duration: 11,
                progress: 0.6, open: true
            },
            {
                id: 2, text: "Task #1", start_date: "03-04-2013", duration: 5,
                progress: 1, open: true, parent: 1
            },
            {
                id: 3, text: "Task #2", start_date: "02-04-2013", duration: 7,
                progress: 0.5, open: true, parent: 1
            },
            {
                id: 4, text: "Task #2.1", start_date: "03-04-2013", duration: 2,
                progress: 1, open: true, parent: 3
            },
            {
                id: 5, text: "Task #2.2", start_date: "04-04-2013", duration: 3,
                progress: 0.8, open: true, parent: 3
            },
            {
                id: 6, text: "Task #2.3", start_date: "05-04-2013", duration: 4,
                progress: 0.2, open: true, parent: 3
            }
        ],
        links: [
            { id: 1, source: 1, target: 2, type: "1" },
            { id: 2, source: 1, target: 3, type: "1" },
            { id: 3, source: 3, target: 4, type: "1" },
            { id: 4, source: 4, target: 5, type: "0" },
            { id: 5, source: 5, target: 6, type: "0" }
        ]
    };

    gantt.init("ganttContainer");
    gantt.parse(tasks1);
})();