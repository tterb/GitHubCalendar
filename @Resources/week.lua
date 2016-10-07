-- ## Information ###################################################################
-- Filename: script.lua
-- Project: GitHubCalendar
-- Author: Brett Stevenson
-- License: BSD 3-Clause
-- Updated: Sept 26, 2016
-- ###############################################################################

-- ## Description ###################################################################
-- A work-in-progress Lua script, intended to improve the performance/functionality of the
-- GitHubCalendar Rainmeter skin by automating the creation and simplifying the modification
-- and update processes.
-- ###############################################################################


-- ## Initialize() ##################################################################
-- The Initialize() function contains code that will be executed when the skin is loaded from within Rainmeter or when major changes are made to the data of the skin.
-- ##############################################################################

function Initialize()
	local dataFile = SKIN:GetVariable('dataPath')
	local file = io.open(dataFile, "r")
	--Loop that writes the values for each block
	for i=1, 7 do
		local squareMeter = "Square" .. i
		SKIN:Bang("!WriteKeyValue", squareMeter, "Meter", "Bar")
		-- Reads values from C# txt file and stores them in lists
		local contribs = file:read("*line")
		local fill = file:read("*line")
		local date = file:read("*line")
		-- Set the X and Y position and meter style
		local remainder = (i-1)%7
		if i == 1 then
			SKIN:Bang("!WriteKeyValue", squareMeter, "MeterStyle", "SquareStyle")
			SKIN:Bang('!WriteKeyValue', squareMeter, 'X', '20')
				SKIN:Bang('!WriteKeyValue', squareMeter, 'Y', '24')
		elseif remainder == 0 then
			SKIN:Bang("!WriteKeyValue", squareMeter, "MeterStyle", "SquareStyle | WeekStartStyle")
		else
			SKIN:Bang("!WriteKeyValue", squareMeter, "MeterStyle", "SquareStyle")
		end
		-- Set the color of the square
		local color = string.sub(fill, 2)
		if color ==  "d6e685" then
			SKIN:Bang('!WriteKeyValue', squareMeter, 'SolidColor', "#Color1#")
		elseif color ==  "8cc665" then
			SKIN:Bang('!WriteKeyValue', squareMeter, 'SolidColor', "#Color2#")
		elseif color ==  "44a340" then
			SKIN:Bang('!WriteKeyValue', squareMeter, 'SolidColor', "#Color3#")
		elseif color == "1e6823"  then
			SKIN:Bang('!WriteKeyValue', squareMeter, 'SolidColor', "#Color4#")
		else
			SKIN:Bang('!WriteKeyValue', squareMeter, 'SolidColor', "#EmptyColor#")
		end
		--Set the date of the square as ToolTipText
		--Set square to display contributions on mouse-over action
		if(string.len(date) >= 8) then
			SKIN:Bang('!WriteKeyValue', squareMeter, 'ToolTipText', date)
			SKIN:Bang('!WriteKeyValue', squareMeter, 'MouseOverAction', '[!SetOption Active Text "' ..contribs ..'"][!UpdateMeter Active][!Redraw]')
		else
			SKIN:Bang('!WriteKeyValue', squareMeter, 'ToolTipText', "")
			SKIN:Bang('!WriteKeyValue', squareMeter, 'MouseOverAction', '[!SetOption Active Text ""][!UpdateMeter Active][!Redraw]')
		end
	end
	file:close()
end



-- #### Update() ####################################################################
-- The Update() function contains code that will be executed when the skin is automatically updated or
-- manually refreshed, reducing the amount of work needed to update the skin.
-- ##############################################################################

function Update( )
	print("Updating")
	local dataPath = SKIN:GetVariable('DataPath')
	local checkFile = io.open(dataPath, "r")
	checkFile:read("*line")
	checkFile:read("*line")
	local weekPath = SKIN:GetVariable('WeekPath')
	local file = io.open(weekPath, "r")
	for i=1, 7 do
		local contribs = file:read("*line")
		local fill = file:read("*line")
		local date = file:read("*line")
		local squareMeter = SKIN:GetMeter('Square'..i)
		local color = string.sub(fill, 2)
		if color ==  "d6e685" then
			SKIN:Bang('!SetOption', squareMeter, 'SolidColor', "#Color1#")
		elseif color ==  "8cc665" then
			SKIN:Bang('!SetOption', squareMeter, 'SolidColor', "#Color2#")
		elseif color ==  "44a340" then
			SKIN:Bang('!SetOption', squareMeter, 'SolidColor', "#Color3#")
		elseif color == "1e6823"  then
			SKIN:Bang('!SetOption', squareMeter, 'SolidColor', "#Color4#")
		else
			SKIN:Bang('!SetOption', squareMeter, 'SolidColor', "#EmptyColor#")
		end
		--Set square to display contributions on mouse-over action and date as tooltip
		if(string.len(date) >= 8) then
			SKIN:Bang('!SetOption', squareMeter, 'ToolTipText', date)
			SKIN:Bang('!SetOption', squareMeter, 'MouseOverAction', '[!SetOption Active Text "'..contribs ..'"][!UpdateMeter Active][!Redraw]')
		else
			SKIN:Bang('!SetOption', squareMeter, 'ToolTipText', "")
			SKIN:Bang('!SetOption', squareMeter, 'MouseOverAction', '[!SetOption Active Text ""][!UpdateMeter Active][!Redraw]')
		end
	end
	return 1;
end
