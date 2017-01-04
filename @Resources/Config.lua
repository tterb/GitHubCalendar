-- ## Information ###################################################################
-- Filename: config.lua
-- Author: Brett Stevenson
-- Project: GitHubCalendar
-- License: BSD 3-Clause
-- Updated: July 31, 2016
-- Description: A work-in-progress Lua script, intended to improve the ease of
-- use of the GitHubCalendar Rainmeter skin by automating much of the user
-- configuration processes. ###############################################################################


function Initialize()
	path = SELF:GetOption('WriteTo')
	Update()
end


function Update()
	local file = io.open(path, "r")
	io.input(file)
	data = io.read("*all")
	io.close(file)
	return tostring(data)
end


function Write()
	local file = io.open(path, "w")
	local username = SELF:GetOption('Input')
	local numWeeks = SELF:GetOption('Weeks')
	io.output(file)
	io.write(tostring(username))
	io.write(tostring(numWeeks))
	io.close(file)
end
