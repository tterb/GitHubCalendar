function Initialize()
	path = SELF:GetOption('WriteTo')
	-- username = SELF:GetVariable('username')
	 file = io.open(path)
	 username = file:read("*all")
	 file.close()
	return username; 
end

function Update()
	-- writePath = SELF:GetOption('WriteTo')
	-- username = SELF:GetVariable('username')
	file = io.open(path)
	-- if not file then
	-- 	print('Config: unable to open file at ' .. writePath)
	-- 	return
	-- end
	username = file:read("*all")
	file.close()
	return username; 
end

function Write() 
	input = SELF:GetOption('Input')
	file = io.open(path, "w")
	file:write(input)
	file.close()
end
