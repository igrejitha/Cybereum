// JavaScript source code
class GanttChart {
    constructor(containerId) {
        this.containerId = containerId;
        this.activities = new Map(); // Changed to a Map for better performance
        this.taskHeight = 30;
        this.timeScale = 'days'; // 'hours', 'days', 'weeks', 'months'
        this.dayWidth = 40;
        this.hourWidth = this.dayWidth / 24;
        this.weekWidth = this.dayWidth * 7;
        this.monthWidth = this.dayWidth * 30;
        this.svg = null;
        this.draggingActivity = null;
        this.draggingStartX = 0;
        this.zoomLevel = 1;
        this.scrollPosition = 0; // The current scroll position
        this.selectedActivity = null;
        this.resizingEdgeSize = 5; // The size of the edge for resizing activities

        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.handleMouseMove = this.handleMouseMove.bind(this);
        this.handleMouseUp = this.handleMouseUp.bind(this);
        this.handleDoubleClick = this.handleDoubleClick.bind(this);
        this.handleWheel = this.handleWheel.bind(this);
        this.handleClick = this.handleClick.bind(this);
        this.handleScroll = this.handleScroll.bind(this);
    }

    addActivity(activity) {
        if (activity.start > activity.end) {
            throw new Error('Start date must be before end date');
        }
        if (this.activities.has(activity.id)) {
            throw new Error('Activity ID must be unique');
        }
        this.activities.set(activity.id, activity);
        this.render();
    }

    removeActivity(activityId) {
        if (!this.activities.has(activityId)) {
            throw new Error('Activity does not exist');
        }
        this.activities.delete(activityId);
        this.render();
    }

    updateActivity(updatedActivity) {
        if (!this.activities.has(updatedActivity.id)) {
            throw new Error('Activity does not exist');
        }
        let activity = this.activities.get(updatedActivity.id);
        Object.assign(activity, updatedActivity);
        this.render();
    }

    setTimeScale(timeScale) {
        this.timeScale = timeScale;
        this.render();
    }

    render() {
        // Get the container element
        let container = document.getElementById(this.containerId);
        container.innerHTML = ''; // Clear the container

        // Create an SVG element
        this.svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        this.svg.setAttribute('width', '100%');
        this.svg.setAttribute('height', this.activities.size * this.taskHeight);
        this.svg.style.backgroundColor = '#0e2841';

        // For each activity, create a rectangle and iterate over tasks and subtasks
        let i = 0;
        for (let [activityId, activity] of this.activities) {
            // Calculate the position and size of the rectangle
            let x, width;
            switch (this.timeScale) {
                case 'hours':
                    x = (activity.start - this.activities.values().next().value.start) / (60 * 60 * 1000) * this.hourWidth * this.zoomLevel;
                    width = (activity.end - activity.start) / (60 * 60 * 1000) * this.hourWidth * this.zoomLevel;
                    break;
                case 'days':
                    x = (activity.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
                    width = (activity.end - activity.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
                    break;
                case 'weeks':
                    x = (activity.start - this.activities.values().next().value.start) / (7 * 24 * 60 * 60 * 1000) * this.weekWidth * this.zoomLevel;
                    width = (activity.end - activity.start) / (7 * 24 * 60 * 60 * 1000) * this.weekWidth * this.zoomLevel;
                    break;
                case 'months':
                    x = (activity.start - this.activities.values().next().value.start) / (30 * 24 * 60 * 60 * 1000) * this.monthWidth * this.zoomLevel;
                    width = (activity.end - activity.start) / (30 * 24 * 60 * 60 * 1000) * this.monthWidth * this.zoomLevel;
                    break;
        }

    // Adjust the position based on the scroll position
    x -= this.scrollPosition;

    let y = i * this.taskHeight;
    let height = this.taskHeight - 2; // Subtract 2 to leave a gap between activities

    // Create the rectangle
    let rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
        rect.setAttribute('x', x);
        rect.setAttribute('y', y);
        rect.setAttribute('width', width);
        rect.setAttribute('height', height);
        rect.setAttribute('fill', activity.color || '#62d0f0');
        rect.setAttribute('id', activity.id);
        rect.setAttribute('class', 'block'); // Add class name

        // Add the rectangle to the SVG element
        this.svg.appendChild(rect);

        // Create a text element for the activity name
        let text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        text.setAttribute('x', x);
        text.setAttribute('y', y + this.taskHeight / 2);
        text.setAttribute('fill', '#d2fbff');
        text.style.fontFamily = this.svg.style.fontFamily;
        text.style.fontSize = this.svg.style.fontSize;
        text.textContent = activity.name;
        text.setAttribute('class', 'text-sf'); // Add class name

        // Add the text element to the SVG element
        this.svg.appendChild(text);

        // Iterate over tasks and subtasks
        for (let task of activity.tasks) {
        // Calculate the position and size of the rectangle for the task
            let taskX = (task.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
            let taskY = i * this.taskHeight;
            let taskWidth = (task.end - task.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
            let taskHeight = this.taskHeight - 2; // Subtract 2 to leave a gap between tasks

        // Create the rectangle for the task
            let taskRect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
            taskRect.setAttribute('x', taskX);
            taskRect.setAttribute('y', taskY);
            taskRect.setAttribute('width', taskWidth);
            taskRect.setAttribute('height', taskHeight);
            taskRect.setAttribute('fill', task.color || '#62d0f0');
            taskRect.setAttribute('id', task.id);

        // Add the rectangle for the task to the SVG element
            this.svg.appendChild(taskRect);

            for (let subtask of task.subtasks) {
        // Calculate the position and size of the rectangle for the subtask
                let subtaskX = (subtask.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
                let subtaskY = i * this.taskHeight;
                let subtaskWidth = (subtask.end - subtask.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel;
                let subtaskHeight = this.taskHeight - 2; // Subtract 2 to leave a gap between subtasks

        // Create the rectangle for the subtask
                let subtaskRect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
                subtaskRect.setAttribute('x', subtaskX);
                subtaskRect.setAttribute('y', subtaskY);
                subtaskRect.setAttribute('width', subtaskWidth);
                subtaskRect.setAttribute('height', subtaskHeight);
                subtaskRect.setAttribute('fill', subtask.color || '#62d0f0');
                subtaskRect.setAttribute('id', subtask.id);

        // Add the rectangle for the subtask to the SVG element
                this.svg.appendChild(subtaskRect);
            }
        }

        // Draw lines to dependent activities
        for (let dependencyId of activity.dependencies) {
            let dependency = this.activities.get(dependencyId);
            if (dependency) {
                let line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                line.setAttribute('x1', x);
                line.setAttribute('y1', y + this.taskHeight / 2);
                line.setAttribute('x2', (dependency.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel);
                line.setAttribute('y2', Array.from(this.activities.values()).indexOf(dependency) * this.taskHeight + this.taskHeight / 2);
                line.setAttribute('stroke', '#d2fbff');
                line.setAttribute('class', 'link'); // Add class name
                this.svg.appendChild(line);
            }
        }
        i++;
    }

    // Add the SVG element to the container
    container.appendChild(this.svg);
        this.svg.setAttribute('id', 'diagram'); // Add ID

        // Add event listeners for user interaction
        this.svg.addEventListener('mousedown', (event) => this.handleMouseDown(event));
        this.svg.addEventListener('mousemove', (event) => this.handleMouseMove(event));
        this.svg.addEventListener('mouseup', (event) => this.handleMouseUp(event));
        this.svg.addEventListener('wheel', (event) => this.handleWheel(event));
        this.svg.addEventListener('click', (event) => this.handleClick(event));
        this.svg.addEventListener('scroll', (event) => this.handleScroll(event)); // Handle scroll events
    }

    clear() {
        this.activities.clear();
        this.render();
    }

    resize(width, height) {
        this.svg.setAttribute('width', width);
        this.svg.setAttribute('height', height);
        this.render();
    }

    changeColor(activityId, color) {
        if (!this.activities.has(activityId)) {
            throw new Error('Activity does not exist');
        }
        let activity = this.activities.get(activityId);
        activity.color = color;
        this.render();
    }

    changeFont(fontFamily, fontSize) {
        this.svg.style.fontFamily = fontFamily;
        this.svg.style.fontSize = fontSize;
        this.render();
    }


    handleMouseDown(event) {
        let activityId = event.target.id;
        let activity = this.activities.get(activityId);

        if (activity) {
            this.draggingActivity = activity;
            this.draggingStartX = event.clientX;
        }

        let rect = event.target;
        let rectX = parseFloat(rect.getAttribute('x'));
        let rectWidth = parseFloat(rect.getAttribute('width'));
        if (Math.abs(event.clientX - rectX) < this.resizingEdgeSize) {
            this.resizingActivity = activity;
            this.resizingEdge = 'left';
        } else if (Math.abs(event.clientX - (rectX + rectWidth)) < this.resizingEdgeSize) {
            this.resizingActivity = activity;
            this.resizingEdge = 'right';
        }
    }

    handleMouseMove(event) {
        if (this.draggingActivity) {
            let dx = event.clientX - this.draggingStartX;
            let deltaDays = Math.round(dx / (this.dayWidth * this.zoomLevel));

            this.draggingActivity.start = new Date(this.draggingActivity.start.getTime() + deltaDays * 24 * 60 * 60 * 1000);
            this.draggingActivity.end = new Date(this.draggingActivity.end.getTime() + deltaDays * 24 * 60 * 60 * 1000);

            let rect = document.getElementById(this.draggingActivity.id);
            rect.setAttribute('x', (this.draggingActivity.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel);

            this.draggingStartX = event.clientX;
        }

        if (this.resizingActivity) {
            let dx = event.clientX - this.draggingStartX;
            let deltaDays = Math.round(dx / (this.dayWidth * this.zoomLevel));

            if (this.resizingEdge === 'left') {
                this.resizingActivity.start = new Date(this.resizingActivity.start.getTime() + deltaDays * 24 * 60 * 60 * 1000);
            } else if (this.resizingEdge === 'right') {
                this.resizingActivity.end = new Date(this.resizingActivity.end.getTime() + deltaDays * 24 * 60 * 60 * 1000);
            }

            let rect = document.getElementById(this.resizingActivity.id);
            if (this.resizingEdge === 'left') {
                rect.setAttribute('x', (this.resizingActivity.start - this.activities.values().next().value.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel);
                rect.setAttribute('width', (this.resizingActivity.end - this.resizingActivity.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel);
            } else if (this.resizingEdge === 'right') {
                rect.setAttribute('width', (this.resizingActivity.end - this.resizingActivity.start) / (24 * 60 * 60 * 1000) * this.dayWidth * this.zoomLevel);
            }

            this.draggingStartX = event.clientX;
        }
    }

    handleMouseUp(event) {
        this.draggingActivity = null;
        this.resizingActivity = null;
    }

    handleDoubleClick(event) {
        let activityId = event.target.id;
        let activity = this.activities.get(activityId);

        if (activity) {
            let name = prompt('Enter a new name for the activity:', activity.name);
            if (name !== null) {
                activity.name = name;
                this.render();
            }
        }
    }

    handleWheel(event) {
        // Update the zoom level based on the wheel movement
        this.zoomLevel += event.deltaY * -0.01;

        // Clamp the zoom level between 0.1 and 2
        this.zoomLevel = Math.min(Math.max(this.zoomLevel, 0.1), 2);

        // Re-render the chart with the new zoom level
        this.render();
    }

    handleClick(event) {
        let activityId = event.target.id;
        let activity = this.activities.get(activityId);

        if (activity) {
            if (this.selectedActivity === activity) {
                this.selectedActivity = null;
                event.target.setAttribute('fill', activity.color || '#62d0f0');
            } else {
                if (this.selectedActivity) {
                    let rect = document.getElementById(this.selectedActivity.id);
                    rect.setAttribute('fill', this.selectedActivity.color || '#62d0f0');
                }

                this.selectedActivity = activity;
                activity.originalColor = activity.color || '#62d0f0';
                event.target.setAttribute('fill', '#d2fbff');
            }
        }
    }

    handleScroll(event) {
        // Update the scroll position
        this.scrollPosition = event.target.scrollLeft;

        // Adjust the position of the activities based on the scroll position
        for (let activity of this.activities) {
            let rect = document.getElementById(activity.id);
            let x = parseFloat(rect.getAttribute('x'));
            rect.setAttribute('x', x - this.scrollPosition);
        }
    }
}
/*
///Example usage - High Level
<script type="text/javascript">
    $(document).ready(function () {
        var ganttChart = new GanttChart('container');

        @foreach (var activity in Model.Activities)
        {
            @:ganttChart.addActivity({
            @:    id: '@activity.Id',
            @:    name: '@activity.Name',
            @:    start: new Date('@activity.Start.ToString("yyyy-MM-dd")'),
            @:    end: new Date('@activity.End.ToString("yyyy-MM-dd")')
            @:});
        }

        ganttChart.render();
    });
</script>
*/